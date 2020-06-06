using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Domain.Configuration;
using DiscordBot.Modules;
using DiscordBot.Modules.Services;
using DiscordBot.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await Task.Delay(-1);
            
            
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(configuration)
                .AddLogging(builder => builder.AddConsole())
                .AddOptions()
                .AddSingleton<HttpClient>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>(x => new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async, LogLevel = LogSeverity.Info }))
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<DiscordLoggingService>()
                .AddTransient<AuthorizationService>();

            serviceCollection.Configure<DiscordSettings>(configuration.GetSection("Discord"));
            serviceCollection.Configure<ImgurSettings>(configuration.GetSection("Imgur"));
            serviceCollection.Configure<Permissions>(configuration.GetSection("Modules:Permissions"));

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