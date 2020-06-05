using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class PingModule : ModuleBase
    {
        [Command("ping")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("Pong!");
        }
    }
}