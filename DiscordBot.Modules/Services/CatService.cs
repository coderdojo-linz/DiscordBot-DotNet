using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public class CatService : ICatService
    {
        private readonly HttpClient _client;

        public CatService(HttpClient client)
        {
            _client = client;
        }

        public async Task<Stream> GetCatAsync()
        {
            HttpResponseMessage message = await _client.GetAsync("https://api.thecatapi.com/v1/images/search?breed_ids=abys");
            string response = await message.Content.ReadAsStringAsync();

            TheCatApiResponse deserialized = JsonConvert.DeserializeObject<TheCatApiResponse[]>(response)[0];

            var url = deserialized.Url;
            HttpResponseMessage message2 = await _client.GetAsync(url);

            return await message2.Content.ReadAsStreamAsync();
        }
    }
}
