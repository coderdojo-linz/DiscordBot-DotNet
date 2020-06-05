using Discord.Commands;
using System.Threading.Tasks;
using System.Net.Http;
using DiscordBot.Domain.WeatherModel;
using Newtonsoft.Json;

namespace DiscordBot.Modules
{
    public class WeatherModule : ModuleBase
    {
        [Command("weather")]
        public async Task TestCommandAsync([Remainder] string smth)
        {
            using (HttpClient c = new HttpClient())
            {
                //Get Weather from the API
                var result = await c.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={smth}&appid=ab2446e861742c6758a49c789d0f4e6a");
                var weather = JsonConvert.DeserializeObject<OpenWeatherMapModel>(await result.Content.ReadAsStringAsync());

                //Get Picture
                var pictureResponse = await c.GetAsync($"http://openweathermap.org/img/wn/{weather.Weather[0].Icon}@2x.png");

                //Send back to Discord Server
                var streamResponse = await pictureResponse.Content.ReadAsStreamAsync();
                await base.Context.Channel.SendFileAsync(streamResponse, "a.png");
            }
        }
    }
}
