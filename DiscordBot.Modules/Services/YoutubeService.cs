using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Configuration;
using DiscordBot.Domain.YouTubeAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiscordBot.Modules.Services
{
    public class YoutubeService
    {
        private readonly YoutubeSettings youtubeSettings;
        private readonly DiscordSocketClient client;
        private readonly ILogger _logger;
        public YoutubeService(
            ILogger<YoutubeSettings> logger,
            DiscordSocketClient client,
            IOptions<YoutubeSettings> youtubeSettings
        )
        {
            this.youtubeSettings = youtubeSettings.Value;
            this.client = client;
            _logger = logger;
        }
        public async Task YoutubeHandler()
        {
            var _guild = client.GetGuild(youtubeSettings.GuildID);
            var _channel = _guild.GetChannel(youtubeSettings.ChannelID) as IVoiceChannel;
            var _channel2 = _guild.GetChannel(youtubeSettings.ChannelID2) as IVoiceChannel;
            var _channel3 = _guild.GetChannel(youtubeSettings.ChannelID3) as IVoiceChannel;

            if (_channel == null)
            {
                return;
            }

            if (!(_guild.GetChannel(youtubeSettings.ChannelID) is IVoiceChannel channel))
            {
                _logger.LogError("Konnte nicht den Kanal als IVoiceChannel casten!");
                return;
            }
            if (_channel2 == null)
            {
                return;
            }

            if (!(_guild.GetChannel(youtubeSettings.ChannelID2) is IVoiceChannel channel2))
            {
                _logger.LogError("Konnte nicht den 2. Kanal als IVoiceChannel casten!");
                return;
            }
            if (_channel == null)
            {
                return;
            }

            if (!(_guild.GetChannel(youtubeSettings.ChannelID3) is IVoiceChannel channel3))
            {
                _logger.LogError("Konnte nicht den 3. Kanal als IVoiceChannel casten!");
                return;
            }


            string query = "https://www.googleapis.com/youtube/v3/channels?part=snippet%2Cstatistics&id=" + youtubeSettings.YTChannelID + "&key=" + youtubeSettings.APIKey;
            HttpClient httpClient = new HttpClient();
            while (true)
            {
                HttpResponseMessage message = await httpClient.GetAsync(query);
                string response = await message.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<Welcome>(response);
                if (data.Items != null && data.Items.Length > 0)
                {
                    var subs = data.Items[0].Statistics.SubscriberCount;
                    await channel.ModifyAsync(property =>
                    {
                        property.Name = $"Abonnenten: {subs}";
                    });
                    _logger.LogInformation($"Abonnenten eingeholt: der User mit der ID {youtubeSettings.YTChannelID} hat {subs} Abonnenten");
                    var views = data.Items[0].Statistics.ViewCount;
                    await channel2.ModifyAsync(property =>
                    {
                        property.Name = $"Views: {views}";
                    });
                    _logger.LogInformation($"Views eingeholt: der User mit der ID {youtubeSettings.YTChannelID} hat {views} Views");
                    var user = data.Items[0].Snippet.Title;
                    await channel3.ModifyAsync(property =>
                    {
                        property.Name = $"YouTube: {user}";
                    });
                    _logger.LogInformation($"YouTube Name eingeholt: der User mit der ID {youtubeSettings.YTChannelID} heißt {user}");
                };

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}
