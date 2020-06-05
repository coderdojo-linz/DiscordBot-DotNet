using CoderDojo_Discordbot.Models.Configuration;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoderDojo_Discordbot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IOptionsMonitor<DiscordSettings> _discordSettings;
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly IServiceProvider _services;

        private char? _messagePrefix = null;

        public CommandHandlingService
        (
            IServiceProvider services,
            CommandService commandService,
            DiscordSocketClient discordSocketClient,
            IOptionsMonitor<DiscordSettings> configurationMonitor,
            ILogger<CommandHandlingService> logger
        )
        {
            _services = services;
            _commands = commandService;
            _discord = discordSocketClient;

            _discordSettings = configurationMonitor;
            _logger = logger;

            InitializePrefix();

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
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

            foreach (var item in AppDomain.CurrentDomain.GetAssemblies().Append(Assembly.Load("DiscordBot.Modules")))
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
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}