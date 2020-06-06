using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CustomStatusModule : ModuleBase
    {
        [Command("status update")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("Pong!");
        }
    }
}