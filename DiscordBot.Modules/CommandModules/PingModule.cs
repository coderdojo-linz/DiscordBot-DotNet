using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules.CommandModules
{
    public class PingModule : ModuleBase
    {
        [Command("ping")]
        public async Task TestCommandAsync()
        {
             await ReplyAsync(":ping_pong: Ping Pong! Der Bot funktioniert!!! Uii");
        }
    }
}
