using Newtonsoft.Json;

namespace DiscordBot.Domain.YouTubeAPI
{
    public partial class PageInfo
    {
        [JsonProperty("totalResults")]
        public long TotalResults { get; set; }
        [JsonProperty("resultsPerPage")]
        public long ResultsPerPage { get; set; }
    }
}
