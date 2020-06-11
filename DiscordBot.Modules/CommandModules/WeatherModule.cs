using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Domain.Abstractions;

namespace DiscordBot.Modules.CommandModules
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
