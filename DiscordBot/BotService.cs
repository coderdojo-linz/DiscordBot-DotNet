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

        public BotService(ILogger<BotService> logger, IOptions<DiscordSettings> discordSettings,
            DiscordSocketClient client, CommandHandlingService commandHandlingService)
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
                await SendStartMessage(client);
            });

            // Here we initialize the logic required to register our commands.
            await commandHandlingService.InitializeAsync();

            await Task.Delay(-1);
        }

        //Warning: Hack
        private async Task SendStartMessage(DiscordSocketClient client)
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
                    _logger.LogInformation($"                        MMM             \n                          MMMM          \n                MMMMMM      MMMMM       \n                M    M       MMMMM      \n                M    M       MMMMMMM    \n                M    M        MMMMMMM   \n                M    M        MMMMMMMM  \n                MMMMMM       MMMMMMMMMM \n                             MMMMMMMMMM \n                            MMMMMMMMMMM \n                          MMMMMMMMMMMMMM\n                        MMMMMMMMMMMMMMMM\n                MMMMMMMMMMMMMMMMMMMMMMMM\n             MMMMMMMMMMMMMMMMMMMMMMMMMM \n            MMMMMMMMMMMMMMMMMMMMMMMMMMM \n           MMMMMMMMMMMMMMMMMMMMMMMMMMMM \n          MMMMMMMM  MMMMMMMMMMMMMMMMMM  \n          MMMMMMMMM MMMMMMMMMMMMMMMMMM  \n          MMMMMMMMM MMMMMMMMMMMMMMMMM   \n          MMMMMMMMM MMMMMMMMMMMMMMMM    \n          MMMMMMMMM MM MMMMMMMMMMM      \n           MMMMMM      MMMMMMMMMM       \n            MMMMMMMMMMMMMMMMMMM         \n              MMMMMMMMMMMMMM            \n                MMMMM                  \n");
                    await messageChannel.SendMessageAsync("Bot started.");
                }
            }
            catch (Exception ex)
            {
                // Guess we dont have this channel anymore ¯\_(ツ)_/¯
                _logger.LogInformation(ex, "Cannot send enter msg");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}