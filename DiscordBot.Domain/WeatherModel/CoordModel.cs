using Newtonsoft.Json;

namespace DiscordBot.Domain.WeatherModel
{
    public partial class CoordModel
    {
        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }
}
