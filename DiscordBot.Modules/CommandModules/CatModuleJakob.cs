using Discord;
using Discord.Commands;

using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DiscordBot.Domain.Configuration;

namespace DiscordBot.Modules
{
    public class CatModuleJakob : ModuleBase
    {
        public CatModuleJakob(IOptions<DiscordSettings> options)
        {
            string prefix = options.Value.CommandPrefix;
            _prefix = prefix;
        }
        string _prefix;
        [Command("catexamples")]
        [Summary("Gibt dir Beispiele für Katzenarten aus")]
        public async Task Catexamples()
        {
            var embed = new EmbedBuilder();
            embed.WithFooter(footer => footer.Text = $"{_prefix}catexamples")
                .WithColor(Color.DarkRed)
                .AddField("Beispiele für Katzen", "Bengal - Beng \n Abyssian - Abys \n Britischer Kurzhaar - bsho \n Toyger - toyg \n sava - Savannah \n Europäischer Burmese - ebur \n Ocikatze - ocic \n Maine Coon - mcoo")
                .WithCurrentTimestamp();
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [Command("cat", RunMode = RunMode.Async)]
        [Summary("Zeigt dir eine süße Katze ^-^")]
        public async Task Cat([Remainder] string input = "nothing")
        {
            int time = 0;
            HttpClient httpClient = new HttpClient();
            string query = "https://api.thecatapi.com/v1/images/search?breed_ids=" + input;
            HttpResponseMessage message = await httpClient.GetAsync(query);
            string response = await message.Content.ReadAsStringAsync();

            if (response == "[]" || input == "nothing")
            {
                if (input == "nothing")
                {
                }
                else
                {
                    await ReplyAsync($"Es konnte keine Katze der Art ``{input}`` gefunden werden!");
                    await Task.Delay(1500);
                    await ReplyAsync("Hier bekommst du trotzdem eine zufällige Katze <:cutecat:720278063409004565>");
                    time = 1000;
                }
                HttpResponseMessage message2 = await httpClient.GetAsync("https://api.thecatapi.com/v1/images/search");
                string response2 = await message2.Content.ReadAsStringAsync();

                TheCatApiResponse deserialized2 = JsonConvert.DeserializeObject<TheCatApiResponse[]>(response2)[0];

                var url2 = deserialized2.Url;
                HttpResponseMessage assyncurl2 = await httpClient.GetAsync(url2);

                var streamResponse2 = await assyncurl2.Content.ReadAsStreamAsync();
                await Task.Delay(time);
                await base.Context.Channel.SendFileAsync(streamResponse2, "cat.jpg");
            }
            else
            {
                TheCatApiResponse deserialized = JsonConvert.DeserializeObject<TheCatApiResponse[]>(response)[0];

                var url = deserialized.Url;
                HttpResponseMessage asyncurl = await httpClient.GetAsync(url);

                var streamResponse = await asyncurl.Content.ReadAsStreamAsync();
                await base.Context.Channel.SendFileAsync(streamResponse, "cat.jpg");
            }
        }
        [Command("catgif")]
        [Alias("gifcat")]
        [Summary("Schickt dir eine animierte Katze ^-^")]
        public async Task Catgif()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync("https://api.thecatapi.com/v1/images/search?mime_types=gif");
            string response = await message.Content.ReadAsStringAsync();

            var deserialized = JsonConvert.DeserializeObject<TheCatApiResponse[]>(response).FirstOrDefault();
            var url = deserialized.Url;

            HttpResponseMessage message2 = await httpClient.GetAsync(url);
            var streamResponse = await message2.Content.ReadAsStreamAsync();
            await base.Context.Channel.SendFileAsync(streamResponse, "cat.gif");
        }
        public class TheCatApiResponse
        {
            [JsonProperty("url")]
            public string Url { get; set; }
        }
    }
}