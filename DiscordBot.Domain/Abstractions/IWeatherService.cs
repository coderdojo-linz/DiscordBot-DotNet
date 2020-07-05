using System.IO;
using System.Threading.Tasks;
using DiscordBot.Domain.Weather.Dto;

namespace DiscordBot.Domain.Abstractions
{
    public interface IWeatherService 
    {
        Task<Stream> GetWeatherStream(string location);
        Task<double> GetTemperature(string location);
        Task<WeatherDto> GetWeatherAsync(string location);
    }
}
