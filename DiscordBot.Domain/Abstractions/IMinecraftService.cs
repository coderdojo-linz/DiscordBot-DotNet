using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public interface IMinecraftService
    {
        Task<string> ExecuteCommandAsync(string command);
    }
}