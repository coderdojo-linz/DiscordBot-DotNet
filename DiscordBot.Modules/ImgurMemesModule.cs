using Discord.Commands;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models.Impl;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Domain.Configuration;

namespace DiscordBot.Modules
{
   public class ImgurMemesModule : ModuleBase
   {
        private ImgurSettings _settings;

        private static List<string> ImageDatabase = new List<string>();
        private static List<string> SentImagesDatabase = new List<string>();

        public ImgurMemesModule(IOptions<ImgurSettings> settings)
        {
            _settings = settings.Value;
        }

        [Command("imgurmeme")]
        public async Task Meme()
        {
            if (ImageDatabase.Count == SentImagesDatabase.Count)
            {
                ImageDatabase.Clear();
                SentImagesDatabase.Clear();
            }

            if (ImageDatabase.Count == 0)
            {
                ImageDatabase = await GetImages();
            }

            foreach (string item in ImageDatabase)
            {
                if (SentImagesDatabase.Contains(item))
                {
                    continue;
                }
                await ReplyAsync(item);
                SentImagesDatabase.Add(item);
                break;
            }
        }

        public async Task<List<string>> GetImages()
        {
            try
            {
                var client = new ImgurClient(_settings.Client_ID, _settings.Client_Secret);

                var gEndPoint = new GalleryEndpoint(client);

                var result = await gEndPoint.SearchGalleryAsync("meme");

                return result
                     .OfType<GalleryAlbum>()
                     .SelectMany(x => x.Images)
                     .Where(x => x.Nsfw.HasValue && !x.Nsfw.Value || x.Nsfw == null)
                     .Select(x => x.Link)
                     .ToList();

            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }

            return new List<string>();
        }
   }
}