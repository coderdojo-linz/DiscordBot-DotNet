using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBot.Domain.Abstractions;
using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Services;
using DiscordBot.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Modules.ReactionModules;
using DiscordBot.Modules.Utils.ReactionBase;
using DiscordBot.Services;
using LibMCRcon.RCon;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WorkerService;

namespace DiscordBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("Environment");

                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var reactionRegistry = new ReactionModuleRegistry(services);
                    //.Register<ReactionTestModule>();

                    services
                        .AddOptions()
                        .AddHttpClient()

                        .AddSingleton<DiscordSocketClient>()
                        .AddSingleton<CommandService, InjectableCommandService>()
                        .AddSingleton<CommandHandlingService>()
                        .AddSingleton<DiscordLoggingService>()
                        .AddSingleton<ReactionModuleRegistry>(reactionRegistry)
                        .AddScoped<IWeatherService, WeatherService>()
                        .AddScoped<ICatService, CatService>()
                        .AddSingleton<MinecraftService>()
                        .AddScoped<ReactionTestModule>()
                        ;

                    services.Configure<DiscordSettings>(hostContext.Configuration.GetSection("Discord"));
                    services.Configure<ImgurSettings>(hostContext.Configuration.GetSection("Imgur"));
                    services.Configure<MinecraftSettings>(hostContext.Configuration.GetSection("Minecraft"));

                    services.AddApplicationInsightsTelemetryWorkerService();

                    services.Configure<CommandServiceConfig>(hostContext.Configuration.GetSection("Discord:CommandService"));

                    services.AddHostedService<BotService>();
                });
    }
}