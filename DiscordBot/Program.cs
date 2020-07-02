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
using System.Net.Http;
using DiscordBot.Domain.CoderDojoInfoModule.Configuration;
using DiscordBot.Domain.CoderDojoInfoModule.ServicesImpl;

namespace DiscordBot
{
    internal class Program
    {
        public static void Main(string[] args) => Host.CreateDefaultBuilder(args)
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
                .AddTransient<HttpClient>(x => x.GetService<IHttpClientFactory>().CreateClient("default"))
                .AddReactionModules()

                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService, InjectableCommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<CommandSuggestionsService>()
                .AddSingleton<DiscordLoggingService>()

                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<ICatService, CatService>()
                .AddSingleton<MinecraftService>()
                .AddTransient<MapBoxStaticMapService>()
                .AddScoped<ICoderDojoAppointmentReaderService, CoderDojoAppointmentReaderService>();

            services.Configure<DiscordSettings>(hostContext.Configuration.GetSection("Discord"));
            services.Configure<ImgurSettings>(hostContext.Configuration.GetSection("Imgur"));
            services.Configure<MinecraftSettings>(hostContext.Configuration.GetSection("Minecraft"));
            services.Configure<JawgSettings>(hostContext.Configuration.GetSection("MapServices:Jawg"));
            services.Configure<MapBoxSettings>(hostContext.Configuration.GetSection("MapServices:MapBox"));
            services.Configure<CDAppointmentSettings>(hostContext.Configuration.GetSection("CoderDojoAppointments"));

            services.AddApplicationInsightsTelemetryWorkerService();

            services.Configure<CommandServiceConfig>(hostContext.Configuration.GetSection("Discord:CommandService"));

            services.AddHostedService<BotService>();
        }
    }
}