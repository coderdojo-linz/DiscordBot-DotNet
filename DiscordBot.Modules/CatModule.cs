using Discord.Commands;

using Newtonsoft.Json;

using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CatModule : ModuleBase
    {
        [Command("cat")]
        public async Task Cat()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync("https://api.thecatapi.com/v1/images/search?breed_ids=abys");
            string response = await message.Content.ReadAsStringAsync();

            TheCatApiResponse deserialized = JsonConvert.DeserializeObject<TheCatApiResponse[]>(response)[0];

            var url = deserialized.Url;
            HttpResponseMessage message2 = await httpClient.GetAsync(url);

            var streamResponse = await message2.Content.ReadAsStreamAsync();
            await base.Context.Channel.SendFileAsync(streamResponse, "cat.jpg");
        }
    }

    public class TheCatApiResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}