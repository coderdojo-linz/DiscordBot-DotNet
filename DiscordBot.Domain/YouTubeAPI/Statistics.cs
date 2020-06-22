using Newtonsoft.Json;

namespace DiscordBot.Domain.YouTubeAPI
{
    public partial class Statistics
    {
        [JsonProperty("viewCount")]
        public long ViewCount { get; set; }
        [JsonProperty("commentCount")]
        public long CommentCount { get; set; }

        [JsonProperty("subscriberCount")]
        public long SubscriberCount { get; set; }
        [JsonProperty("hiddenSubscriberCount")]
        public bool HiddenSubscriberCount { get; set; }
        [JsonProperty("videoCount")]
        public long VideoCount { get; set; }
    }
}
