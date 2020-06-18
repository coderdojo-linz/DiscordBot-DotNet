using Newtonsoft.Json;

namespace DiscordBot.Domain.Weather.OpenWeatherMapModel
{
    public partial class WindModel
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public long Deg { get; set; }
    }
}
