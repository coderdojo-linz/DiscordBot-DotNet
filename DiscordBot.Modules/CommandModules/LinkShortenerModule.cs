using Discord;
using Discord.Commands;

using DiscordBot.Domain.Abstractions;

using System;
using System.IO;
using System.Threading.Tasks;

using DiscordBot.Modules.Services;

namespace DiscordBot.Modules.CommandModules
{
    public class LinkShortenerModule : ModuleBase
    {
        private readonly LinkShortenerService _linkShortenerService;
        private readonly LinkShortenerSettings _settings;

        public LinkShortenerModule(LinkShortenerService linkShortenerService, LinkShortenerSettings settings)
        {
            _linkShortenerService = linkShortenerService;
            _settings = settings;
        }

        [Command("update")]
        public async Task UpdateLinkAsync(string id, [Remainder]string link)
        {
            await Context.Channel.SendMessageAsync($"Updating {id} to {link}!");
            await _linkShortenerService.UpdateUrlAsync(id, _settings.AccessKey, link);
            await Context.Channel.SendMessageAsync($"Update complete!");
        }

        // [Command("weather-legacy")]
        // public async Task GetWeatherLegacyAsync([Remainder] string location)
        // {
        //     //Get Values
        //     var WeatherStream = await _weatherService.GetWeatherStream(location);
        //     double temperature = await _weatherService.GetTemperature(location);

        //     //Check if returned Values are valid
        //     if (WeatherStream == null)
        //     {
        //         await Context.Channel.SendMessageAsync("Ungültiger Ort!");
        //         return;
        //     }

        //     if (WeatherStream.CanSeek)
        //     {
        //         WeatherStream.Seek(0, SeekOrigin.Begin);
        //     }

        //     //Answer
        //     await Context.Channel.SendFileAsync(WeatherStream, $"{location}.png", temperature.ToString());
        // }

    //     [Command("weather")]
    //     public async Task GetWeatherAsync([Remainder] string location)
    //     {
    //         var weather = await _weatherService.GetWeatherAsync(location);
    //         if (weather == null)
    //         {
    //             await ReplyAsync($"Für `{location}` wurde kein Wetter gefunden!");
    //             return;
    //         }

    //         var eb = new EmbedBuilder()
    //             .WithThumbnailUrl(weather.ThumbnailUrl)
    //             .WithTitle(weather.Main)
    //             .WithDescription(weather.Description)
    //             .WithColor(GetEmbedColor((float)weather.TempAsDouble))
    //             .AddField("Temparatur", weather.Temparature, true)
    //             .AddField("RealFeel", weather.RealFeelTemp, true)
    //             .AddField("\u200b", "\u200b", false)
    //             .AddField("Luftfeuchtigkeit", weather.Humidity, true)
    //             .AddField("Luftdruck", weather.Pressure, true)
    //             ;

    //         await ReplyAsync(embed: eb.Build());
    //     }

    //     private Color GetEmbedColor(float temp)
    //     {
    //         float minTemp = -5;
    //         float maxTemp = 40;
    //         float span = maxTemp - minTemp;

    //         temp = (float)Limit(minTemp, temp, maxTemp);
    //         float partOfColor = (float)Math.Round((temp - minTemp) / span, digits: 3);
    //         var startColor = (r: 52, g: 152, b: 219);
    //         var endColor = (r: 231, g: 76, b: 60);

    //         float red = CalcColorPart(startColor.r, endColor.r, partOfColor);
    //         float green = CalcColorPart(startColor.g, endColor.g, partOfColor);
    //         float blue = CalcColorPart(startColor.b, endColor.b, partOfColor);
            
    //         return new Color((int)Limit(0, Math.Round(red), 255), (int)Limit(0, Math.Round(green), 255), (int)Limit(0, Math.Round(blue), 255));
    //     }

    //     private double Limit(double min, double given, double max)
    //     {
    //         return Math.Max(min, Math.Min(given, max));
    //     }

    //     private float CalcColorPart(float value1, float value2, float partOfColor)
    //     {
    //         float span = Math.Abs(value2 - value1);
    //         float partOfSpan = partOfColor * span;
    //         if (value2 > value1)
    //         {
    //             return value1 + partOfSpan;
    //         }
    //         else
    //         {
    //             return value1 - partOfSpan;
    //         }
    //     }
    }
}