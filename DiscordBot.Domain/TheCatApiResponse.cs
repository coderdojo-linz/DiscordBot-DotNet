using Newtonsoft.Json;

namespace DiscordBot.Modules.Services
{
    public class TheCatApiResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
