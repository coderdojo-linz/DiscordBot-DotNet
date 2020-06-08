using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using DiscordBot.Domain.WeatherModel;
using Newtonsoft.Json;
using DiscordBot.Domain.Abstractions;
using System.IO;

namespace DiscordBot.Modules
{
    public class WeatherModule : ModuleBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherModule(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [Command("weather")]
        public async Task GetWeatherAsync([Remainder] string location)
        {
            var catStream = await _weatherService.GetWeatherStream(location);
            if (catStream.CanSeek)
            {
                catStream.Seek(0, SeekOrigin.Begin);
            }

            await Context.Channel.SendFileAsync(catStream, "cat.png");

        }
    }
}
