using Discord.Commands;

using DiscordBot.Domain.Configuration;
using DiscordBot.Domain.IpInfo;
using DiscordBot.Modules.Services;

using Flurl.Http;

using Microsoft.Extensions.Options;

using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    [Group("ip")]
    public class IpInfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly MapBoxStaticMapService _mapBoxStaticMapService;
        private JawgSettings _jawgSettings;

        public IpInfoModule
        (
            IOptions<JawgSettings> jawgSettings,
            MapBoxStaticMapService mapBoxStaticMapService
        )
        {
            _mapBoxStaticMapService = mapBoxStaticMapService;
            _jawgSettings = jawgSettings.Value;
        }

        [Command("location")]
        public async Task Location(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                await ReplyAsync("Ungültiges IP-Format!");
                return;
            }

            var ipInfo = await $"http://ip-api.com/json/{ipAddress}".GetJsonAsync<IpApiResult>();
            if (ipInfo == null)
            {
                await ReplyAsync($"IP-Adresse {ipAddress} konnte nicht gefunden werden!");
                return;
            }

            var staticMap = await _mapBoxStaticMapService.GetImageStream(ipInfo.Lat, ipInfo.Lon);
            // await GetStaticImageUrlMapBox(ipInfo).GetStreamAsync();
            await base.Context.Channel.SendFileAsync(staticMap, "map.png", $"{ipInfo.Country} - {ipInfo.City}");
        }

        private string GetStaticImageUrlMapBox(IpApiResult ipInfo)
        {
            var culture = new CultureInfo("en-US");
            var apiKey = "pk.eyJ1IjoibWFwYm94IiwiYSI6ImNpejY4M29iazA2Z2gycXA4N2pmbDZmangifQ.-g_vE53SD2WrJ6tFX7QHmA";
            var position = $"{ipInfo.Lon.ToString(culture)},{ipInfo.Lat.ToString(culture)}";

            return $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-l({position})/{position},14/720x480?access_token={apiKey}";
        }

        private string GetStaticImageUrl(IpApiResult ipInfo)
        {
            const string staticElements = "zoom=12&size=720x480&layer=jawg-sunny&format=png";
            var culture = new CultureInfo("en-US");

            var position = $"{ipInfo.Lat.ToString(culture)},{ipInfo.Lon.ToString(culture)}";
            var arguments = string.Join("&",
                $"center={position}",
                $"access-token={_jawgSettings.ApiKey}",
                $"marker=color:ff0000,size:small,label:A%7C{position}",
                staticElements
            );

            return $"https://api.jawg.io/static?{arguments}";
        }
    }
}