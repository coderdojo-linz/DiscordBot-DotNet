using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Configuration;
using DiscordBot.Services.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
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
                Console.WriteLine("!Connected");
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
        private static async Task SendStartMessage(DiscordSocketClient client)
        {
            try
            {
                var guild = client.GetGuild(718465629656449125);
                var channel = guild.GetChannel(718465629656449128);

                if (channel is IMessageChannel messageChannel)
                {
                    await messageChannel.SendMessageAsync("Bot started.");
                }
            }
            catch (Exception)
            {
                // Guess we dont have this channel anymore ¯\_(ツ)_/¯
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
