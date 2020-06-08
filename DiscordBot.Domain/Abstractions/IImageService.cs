
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Domain.Abstractions
{
    //public interface ICatService : IImageService
    //{
    //}

    public interface IWeatherService 
    {
        Task<Stream> GetWeatherStream(string location);

    }

    public interface IImageService
    {
        Task<Stream> GetImageStreamAsync();
    }

}
