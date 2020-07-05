using Discord;
using Discord.WebSocket;

using DiscordBot.Domain.Configuration;
using DiscordBot.Services.Base;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
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
            _logger.LogInformation("Mit Discord verbinden...");
            await client.LoginAsync(TokenType.Bot, discordSettings.Token);
            await client.StartAsync();

            while (client.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(300);
            }

            _logger.LogInformation("Verbunden!");

            _ = Task.Run(async () =>
            {
                await SendBotSpamMessage(client, "Bot wurde gestartet.");
            });

            // Here we initialize the logic required to register our commands.
            await commandHandlingService.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await SendBotSpamMessage(client, "Bot stoppt...");

            _logger.LogInformation("Ausloggen.");
            await client.LogoutAsync();
            _logger.LogInformation("Verbindung trennen.");
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
                    _logger.LogInformation($"Nachricht konnte nich gesendet werden: Kanal {id} nicht gefunden!");
                    return;
                }

                if (botChannel is IMessageChannel messageChannel)
                {
                    _logger.LogInformation($"Nachricht wird an #{botChannel.Name} - {botChannel.Guild.Name} gesendet.");

                    await messageChannel.SendMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Guess we dont have this channel anymore ¯\_(ツ)_/¯
                _logger.LogInformation($"Nachricht konnte nich gesendet werden: {ex}");
            }
        }
    }
}