using Newtonsoft.Json;

namespace DiscordBot.Domain.YouTubeAPI
{
    public partial class Welcome
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("pageInfo")]
        public PageInfo PageInfo { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }
    }
}
