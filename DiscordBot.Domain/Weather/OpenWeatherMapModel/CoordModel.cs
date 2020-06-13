using Newtonsoft.Json;

namespace DiscordBot.Domain.Weather.OpenWeatherMapModel
{
    public partial class CoordModel
    {
        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }
}
