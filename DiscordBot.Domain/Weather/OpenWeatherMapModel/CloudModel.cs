using Newtonsoft.Json;

namespace DiscordBot.Domain.Weather.OpenWeatherMapModel
{
    public partial class CloudsModel
    {
        [JsonProperty("all")]
        public long All { get; set; }
    }
}
