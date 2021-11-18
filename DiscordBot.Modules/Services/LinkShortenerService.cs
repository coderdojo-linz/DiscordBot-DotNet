using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CDWPlanner.Model;

namespace CDWPlanner.Services
{
    public class LinkShortenerService
    {
        private HttpClient _client;

        public LinkShortenerService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("linkshortener");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="desiredId">the id the resulting url should be </param>
        /// <param name="accessKey">the access key to edit the link</param>
        /// <param name="urlToShort">The longer url</param>
        /// <returns></returns>
        public async Task<ShortenedLink> ShortenUrl(string desiredId, string accessKey, string urlToShort)
        {
            var result = await _client.PostAsync("Shortener", new JsonContent(new
            {
                Id = desiredId,
                AccessKey = accessKey,
                Url = urlToShort
            }));
            string content = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                content = await result.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<ShortenedLinkResponse<ShortenedLink>>(content);
                return deserializedContent.Success
                    ? deserializedContent.Data
                    : throw new Exception($"The linkshortener service returned '{deserializedContent.Message}'");
            }

            try
            {
                content = await result.Content.ReadAsStringAsync();

                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    var response = JsonConvert.DeserializeObject<ShortenedLinkResponse<ShortenedLink>>(content);
                    if (string.Equals(response.Message, "Id already exists", StringComparison.OrdinalIgnoreCase))
                    {
                        //Updating

                        return await UpdateUrlAsync(desiredId, accessKey, urlToShort);
                    }
                    else
                    {
                        throw new Exception($"The linkshortener service returned '{response.Message}'");
                    }
                }
                
                throw new Exception($"The linkshortener service returned '{content}'");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ShortenedLink> UpdateUrlAsync(string linkId, string accessKey, string urlToShort)
        {
            var result = await _client.PutAsync($"Shortener/{linkId}", new JsonContent(new
            {
                AccessKey = accessKey,
                Url = urlToShort
            }));

            string content = string.Empty;
            content = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ShortenedLinkResponse<ShortenedLink>>(content).Data;
            }

            throw new Exception($"The linkshortener service returned '{result}'");
        }
    }
    
    public class LinkShortenerSettings
    {
        public string AccessKey { get; }

        public LinkShortenerSettings(string accessKey)
        {
            AccessKey = accessKey;
        }
    }
}