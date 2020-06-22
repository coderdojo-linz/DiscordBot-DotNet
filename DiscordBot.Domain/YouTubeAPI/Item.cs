using Newtonsoft.Json;

namespace DiscordBot.Domain.YouTubeAPI
{
    public partial class Item
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }
    }
}
