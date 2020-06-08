using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public interface ICatService
    {
        Task<Stream> GetCatAsync();
    }
}