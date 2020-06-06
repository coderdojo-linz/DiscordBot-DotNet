using Newtonsoft.Json;

namespace DiscordBot.Domain.WeatherModel
{
    public partial class OpenWeatherMapModel
    {
        [JsonProperty("coord")]
        public CoordModel Coord { get; set; }

        [JsonProperty("weather")]
        public WeatherModel[] Weather { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("main")]
        public MainModel Main { get; set; }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("wind")]
        public WindModel Wind { get; set; }

        [JsonProperty("clouds")]
        public CloudsModel Clouds { get; set; }

        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonProperty("sys")]
        public SysModel Sys { get; set; }

        [JsonProperty("timezone")]
        public long Timezone { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cod")]
        public long Cod { get; set; }
    }
}