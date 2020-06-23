using Newtonsoft.Json;
using System;

namespace DiscordBot.Domain.YouTubeAPI
{
    public partial class Snippet
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("customUrl")]
        public string CustomUrl { get; set; }

        [JsonProperty("publishedAt")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonProperty("localized")]
        public Localized Localized { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
}