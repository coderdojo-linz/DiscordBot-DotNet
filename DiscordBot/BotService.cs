using Discord;
using Discord.WebSocket;

using DiscordBot.Domain.Configuration;
using DiscordBot.Services.Base;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class BotService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly DiscordSettings discordSettings;
        private readonly YoutubeSettings youtubeSettings;
        private readonly DiscordSocketClient client;
        private readonly CommandHandlingService commandHandlingService;

        public BotService
        (
            ILogger<BotService> logger,
            IOptions<DiscordSettings> discordSettings,
            DiscordSocketClient client,
            CommandHandlingService commandHandlingService,
            IOptions<YoutubeSettings> youtubeSettings
        )
        {
            _logger = logger;
            this.discordSettings = discordSettings.Value;
            this.client = client;
            this.commandHandlingService = commandHandlingService;
            this.youtubeSettings = youtubeSettings.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await client.LoginAsync(TokenType.Bot, discordSettings.Token);
            await client.StartAsync();

            while (client.ConnectionState != ConnectionState.Connected)
            {
                _logger.LogInformation("!Connected");
                await Task.Delay(500);
            }

            _ = Task.Run(async () =>
            {
                await SendBotSpamMessage(client, "Bot started.");
            });

            // Here we initialize the logic required to register our commands.
            await commandHandlingService.InitializeAsync();

            //await Task.Delay(-1);
            _ = Task.Run(async () => await this.YoutubeHandler());


        }

        
        public async Task YoutubeHandler()
        {
            int _counter = 1;
            while (true)
            {
                var _Guild = client.GetGuild(youtubeSettings.GuildID);
                var _Channel = _Guild.GetChannel(youtubeSettings.ChannelID);
                await _Channel.ModifyAsync(prop => prop.Name = $"Hallo :D {_counter}");
                _counter += 1;
                _logger.LogInformation($"{ _counter}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await SendBotSpamMessage(client, "Stopping bot...");

            _logger.LogInformation("Attempting graceful stop");
            _logger.LogInformation("Logging out");
            await client.LogoutAsync();
            _logger.LogInformation("Disconnecting");
            await client.StopAsync();
            client.Dispose();
        }

        //Warning: Hack
        private async Task SendBotSpamMessage(DiscordSocketClient client, string message)
        {
            try
            {
                var dojo = client.Guilds.FirstOrDefault(x => x.Name?.Contains("CoderDojo Austria") ?? false);
                if (dojo == null)
                {
                    _logger.LogInformation("Cannot send enter msg: Dojo server not found!");
                    return;
                }
                var spamChannel = dojo.Channels.FirstOrDefault(x => x.Name == "bot-spam");
                if (spamChannel == null)
                {
                    _logger.LogInformation("Cannot send enter msg: Spam channel not found!");
                    return;
                }

                if (spamChannel is IMessageChannel messageChannel)
                {
                    _logger.LogInformation($"Sending msg to {dojo.Name} - {spamChannel.Name}");

                    await messageChannel.SendMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Guess we dont have this channel anymore ¯\_(ツ)_/¯
                _logger.LogInformation(ex, "Cannot send enter msg");
            }
        }
    }
}