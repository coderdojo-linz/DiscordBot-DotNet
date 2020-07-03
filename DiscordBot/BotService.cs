using Discord;
using Discord.WebSocket;

using DiscordBot.Domain.Configuration;
using DiscordBot.Services.Base;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class BotService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly DiscordSettings discordSettings;
        private readonly DiscordSocketClient client;
        private readonly CommandHandlingService commandHandlingService;

        public BotService
        (
            ILogger<BotService> logger, 
            IOptions<DiscordSettings> discordSettings,
            DiscordSocketClient client, 
            CommandHandlingService commandHandlingService
        )
        {
            _logger = logger;
            this.discordSettings = discordSettings.Value;
            this.client = client;
            this.commandHandlingService = commandHandlingService;
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
                var id = discordSettings.WelcomeMessageChannelId;
                var botChannel = (IGuildChannel) client.GetChannel(id);

                if (botChannel == null)
                {
                    _logger.LogInformation($"Cannot send enter msg: Channel {id} not found!");
                    return;
                }

                if (botChannel is IMessageChannel messageChannel)
                {
                    _logger.LogInformation($"Sending msg to {botChannel.Guild.Name} - {botChannel.Name}");

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