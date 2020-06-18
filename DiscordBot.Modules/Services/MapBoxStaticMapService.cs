using DiscordBot.Domain.Configuration;

using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using ISSize = SixLabors.ImageSharp.Size;
using Size = System.Drawing.Size;

namespace DiscordBot.Modules.Services
{
    public class MapBoxStaticMapService
    {
        private readonly HttpClient _client;
        private string _apiKey;

        public MapBoxStaticMapService(HttpClient client, IOptions<MapBoxSettings> mapBoxSettings)
        {
            _client = client;
            _apiKey = mapBoxSettings.Value.ApiKey;
        }

        public async Task<Stream> GetImageStream(double latitude, double longitude, Size? size = default)
        {
            var realSize = size != null ? new ISSize(size.Value.Width, size.Value.Height) : new ISSize(1280, 720);

            var virtualSize = new Size(realSize.Width, (int)Math.Ceiling(realSize.Height * 1.05));

            var url = GetImageUrl(latitude, longitude, virtualSize);
            var rawImage = await _client.GetByteArrayAsync(url);
            var image = Image.Load(rawImage);
            image.Mutate(x => x.Crop(new Rectangle(Point.Empty, realSize)));

            var ms = new MemoryStream();
            image.Save(ms, PngFormat.Instance);
            ms.Position = 0;
            return ms;
        }

        public string GetImageUrl(double latitude, double longitude, Size? size = default)
        {
            var rs = size ?? new Size(1280, 720);

            var sizeString = $"{rs.Width}x{rs.Height}";

            var culture = new CultureInfo("en-US");

            var position = $"{longitude.ToString(culture)},{latitude.ToString(culture)}";
            //{sizeString}@2x
            return $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-l({position})/{position},14/{sizeString}?access_token={_apiKey}";
        }
    }
}