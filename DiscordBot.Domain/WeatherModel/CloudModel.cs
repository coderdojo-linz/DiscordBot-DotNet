using Newtonsoft.Json;

namespace DiscordBot.Domain.WeatherModel
{
    public partial class CloudsModel
    {
        [JsonProperty("all")]
        public long All { get; set; }
    }
}
