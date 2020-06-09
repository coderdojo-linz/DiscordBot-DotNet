using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBot.Domain.Abstractions;
using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Services;
using DiscordBot.Services.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            var services = ConfigureServices(configuration);

            await RunBotAsync(services);
            Console.WriteLine("Bot Stopped. Bye :)");
        }

        private static async Task RunBotAsync(IServiceProvider services)
        {
            //Ensure that the logger is running
            services.GetService<DiscordLoggingService>();

            var discordSettings = services.GetService<IOptions<DiscordSettings>>().Value;
            var client = services.GetRequiredService<DiscordSocketClient>();

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
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(-1);
        }

        //Warning: Hack
        private static async Task SendStartMessage(DiscordSocketClient client)
        {
            try
            {
                var guild = client.GetGuild(704990064039559238);
                var channel = guild.GetChannel(719867879054377092);

                if (channel is IMessageChannel messageChannel)
                {
                    await messageChannel.SendMessageAsync("Bot started.");
                }
            }
            catch (Exception e)
            {
                // Guess we dont have this channel anymore ¯\_(ツ)_/¯
            }
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(configuration)
                .AddLogging(builder => builder.AddConsole())
                .AddOptions()
                .AddHttpClient()
                .AddTransient(x => x.GetService<IHttpClientFactory>().CreateClient("Default"))

                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<ICatService, CatService>()

                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<DiscordLoggingService>()
                ;

            serviceCollection.Configure<DiscordSettings>(configuration.GetSection("Discord"));
            serviceCollection.Configure<ImgurSettings>(configuration.GetSection("Imgur"));

            return serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("Environment");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"configuration.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}