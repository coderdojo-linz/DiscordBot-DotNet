using Discord.Commands;

using DiscordBot.Domain.Configuration;
using DiscordBot.Domain.IpInfo;

using Flurl.Http;

using Microsoft.Extensions.Options;

using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    [Group("ip")]
    public class IpInfoModule : ModuleBase<SocketCommandContext>
    {
        private JawgSettings _jawgSettings;

        public IpInfoModule(IOptions<JawgSettings> jawgSettings)
        {
            _jawgSettings = jawgSettings.Value;
        }

        [Command("location")]
        public async Task Location(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                await ReplyAsync("Invalid ip format :(");
                return;
            }
            

            var ipInfo = await $"http://ip-api.com/json/{ipAddress}".GetJsonAsync<IpApiResult>();
            if (ipInfo == null)
            {
                await ReplyAsync($"Cannot find ip {ipAddress}");
                return;
            }

            var staticMap = await GetStaticImageUrl(ipInfo).GetStreamAsync();
            await base.Context.Channel.SendFileAsync(staticMap, "map.png", $"{ipInfo.Country} - {ipInfo.City}");
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