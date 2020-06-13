namespace DiscordBot.Domain.Weather.Dto
{
    public class WeatherDto
    {
        public string ThumbnailUrl { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Temparature { get; set; }
    }
}