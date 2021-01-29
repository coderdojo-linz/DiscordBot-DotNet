using System.Threading.Tasks;

namespace DiscordBot.Database
{
    public interface IAsyncInitializable
    {
        int Priority => 0;

        Task InitializeAsync();
    }
}