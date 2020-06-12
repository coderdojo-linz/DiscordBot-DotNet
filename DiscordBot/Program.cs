using Discord.Commands;
using Discord.WebSocket;

using DiscordBot.Domain.Abstractions;
using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Services;
using DiscordBot.Modules.Utils.ReactionBase;
using DiscordBot.Services;
using DiscordBot.Services.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.IO;

namespace DiscordBot
{
    internal class Program
    {
        public static void Main(string[] args)  => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(GetConfiguration)
            .ConfigureServices(ConfigureServices)
            .Build().Run();

        private static void GetConfiguration(IConfigurationBuilder builder)
        {
            var environmentName = Environment.GetEnvironmentVariable("Environment");

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddOptions()
                .AddHttpClient()
                .AddReactionModules()

                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService, InjectableCommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<DiscordLoggingService>()
              
                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<ICatService, CatService>()
                .AddSingleton<MinecraftService>();

            services.Configure<DiscordSettings>(hostContext.Configuration.GetSection("Discord"));
            services.Configure<ImgurSettings>(hostContext.Configuration.GetSection("Imgur"));
            services.Configure<MinecraftSettings>(hostContext.Configuration.GetSection("Minecraft"));

            services.AddApplicationInsightsTelemetryWorkerService();

            services.Configure<CommandServiceConfig>(hostContext.Configuration.GetSection("Discord:CommandService"));

            services.AddHostedService<BotService>();
        }
    }
}