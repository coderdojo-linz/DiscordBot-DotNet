using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Utils.ReactionBase;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Services.Base
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IOptionsMonitor<DiscordSettings> _discordSettings;
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IServiceProvider _services;

        private char? _messagePrefix = null;

        private readonly ConcurrentDictionary<ulong, IServiceScope> _scopes = new ConcurrentDictionary<ulong, IServiceScope>();

        public CommandHandlingService
        (
            IServiceProvider services,
            CommandService commandService,
            DiscordSocketClient discordSocketClient,
            IOptionsMonitor<DiscordSettings> configurationMonitor,
            ILogger<CommandHandlingService> logger,
            TelemetryClient telemetryClient
        )
        {
            _services = services;
            _commands = commandService;
            _discord = discordSocketClient;

            _discordSettings = configurationMonitor;
            _logger = logger;
            _telemetryClient = telemetryClient;
            InitializePrefix();

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;

            _discord.ReactionAdded += (x, y, z) => ReactionAction(x, y, z, ReactionType.Added);
            _discord.ReactionRemoved += (x, y, z) => ReactionAction(x, y, z, ReactionType.Removed);
            _discord.ReactionsCleared += (x, y) => ReactionAction(x, y, null, ReactionType.Cleared);
        }

        private async Task ReactionAction
        (
            Cacheable<IUserMessage, ulong> userMessage,
            ISocketMessageChannel messageChannel,
            SocketReaction reaction,
            ReactionType reactionType
        )
        {
            using (var scope = _services.CreateScope())
            {
                var registry = scope.ServiceProvider.GetService<ReactionModuleRegistry>();
                if (registry == null)
                {
                    _logger.LogWarning($"The {nameof(ReactionModuleRegistry)} is unconfigured");
                    return;
                }

                var types = registry.GetRegisteredTypes(reaction.Emote.Name);
                if (!types.Any())
                {
                    return;
                }

                var context = new ReactionContext(userMessage, messageChannel, reaction, reactionType);

                foreach (var type in types)
                {
                    if (!(scope.ServiceProvider.GetService(type) is ReactionModuleBase module))
                    {
                        _logger.LogWarning($"Invalid type in {nameof(ReactionModuleRegistry)} found: '{type.FullName}'");
                        continue;
                    }

                    module.Context = context;
                    try
                    {
                        if (await module.ExecuteAsync())
                        {
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, $"ReactionHandler '{module.GetType().FullName}' returned exception!");
                        throw;
                    }
                }
            }
        }

        private void InitializePrefix()
        {
            UpdateMessagePrefix(_discordSettings.CurrentValue.CommandPrefix);
            _discordSettings.OnChange(x => UpdateMessagePrefix(x.CommandPrefix));
        }

        private void UpdateMessagePrefix(string prefix)
        {
            var prefixLength = string.IsNullOrEmpty(prefix) ? 0 : prefix.Length;
            var prefixValid = prefixLength == 1;
            if (prefixValid)
            {
                _messagePrefix = prefix[0];
                return;
            }

            if (!_messagePrefix.HasValue)
            {
                _logger.LogWarning($"Prefix has invalid length ({prefixLength}). Defaulting to '!'");
                _messagePrefix = '!';
            }
            else
            {
                _logger.LogWarning($"Prefix has invalid length ({prefixLength})");
            }
            return;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.

            foreach (var item in AppDomain.CurrentDomain.GetAssemblies().Append(Assembly.Load("DiscordBot.Modules")).Distinct())
            {
                var modules = (await _commands.AddModulesAsync(item, _services)).ToList();
                if (modules.Count == 0)
                {
                    continue;
                }

                _logger.Log(LogLevel.Information, $"Added {modules.Count} modules. ({string.Join('|', modules.Select(m => m.Name))})");
            }

            _logger.Log(LogLevel.Information, $"Modules initialized");
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            //if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            if (!_messagePrefix.HasValue)
            {
                _logger.Log(LogLevel.Information, $"No prefix found");
                return;
            }

            if (!message.HasCharPrefix(_messagePrefix.Value, ref argPos) && !(message.Channel is IPrivateChannel))
            {
                return;
            }

            var context = new SocketCommandContext(_discord, message);

            //The discordNET client doesnt create a scope for us, so we have to care about it
            var scope = _services.CreateScope();
            _scopes[message.Id] = scope;

            using var operation = _telemetryClient.StartOperation<RequestTelemetry>(context.Message.Content);
            operation.Telemetry.Properties.Add("User", context.User.Username);

            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            var result = await _commands.ExecuteAsync(context, argPos, scope.ServiceProvider);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,

            operation.Telemetry.Success = result.IsSuccess;
            _telemetryClient.StopOperation(operation);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (_scopes.TryRemove(context.Message.Id, out var scopeToDispose))
            {
                scopeToDispose.Dispose();
            }

            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                _logger.LogInformation($"Command not recognized: {context?.Message?.Content ?? "[NOT_FOUND]" }");
                return;
            }

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            if (result is ExecuteResult executeResult && executeResult.Exception != null)
            {
                _logger.LogError(executeResult.Exception, executeResult.Exception.Message);
            }

            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}