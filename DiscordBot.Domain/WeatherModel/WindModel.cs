using Newtonsoft.Json;

namespace DiscordBot.Domain.WeatherModel
{
    public partial class WindModel
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public long Deg { get; set; }
    }
}
