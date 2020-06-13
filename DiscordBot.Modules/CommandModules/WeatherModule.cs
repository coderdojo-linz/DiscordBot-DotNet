using Discord;
using Discord.Commands;

using DiscordBot.Domain.Abstractions;

using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class WeatherModule : ModuleBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherModule(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [Command("weather-legacy")]
        public async Task GetWeatherLegacyAsync([Remainder] string location)
        {
            //Get Values
            var WeatherStream = await _weatherService.GetWeatherStream(location);
            double temperature = await _weatherService.GetTemperature(location);

            //Check if returned Values are valid
            if (WeatherStream == null)
            {
                await Context.Channel.SendMessageAsync("Location invalid");
                return;
            }

            if (WeatherStream.CanSeek)
            {
                WeatherStream.Seek(0, SeekOrigin.Begin);
            }

            //Answer
            await Context.Channel.SendFileAsync(WeatherStream, $"{location}.png", temperature.ToString());
        }

        [Command("weather")]
        public async Task GetWeatherAsync([Remainder] string location)
        {
            var weather = await _weatherService.GetWeatherAsync(location);

            var eb = new EmbedBuilder()
                .WithThumbnailUrl(weather.ThumbnailUrl)
                .WithTitle(weather.Main)
                .WithDescription(weather.Description)
                .AddField("Temparatur", weather.Temparature);

            await ReplyAsync(embed: eb.Build());
        }
    }
}