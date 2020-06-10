using DiscordBot.Domain.Abstractions;
using DiscordBot.Domain.WeatherModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
        }



        public async Task<Stream> GetWeatherStream(string location)
        {
            //Get Weather from the API
            var result = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=ab2446e861742c6758a49c789d0f4e6a");
            var weather = JsonConvert.DeserializeObject<OpenWeatherMapModel>(await result.Content.ReadAsStringAsync());

            //Get Picture
            var pictureResponse = await _httpClient.GetAsync($"http://openweathermap.org/img/wn/{weather.Weather[0].Icon}@2x.png");

            //Send back to Discord Server
            return await pictureResponse.Content.ReadAsStreamAsync();


        }
    }
}
