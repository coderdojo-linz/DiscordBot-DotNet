using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("status")]
    public class CustomStatusModule : ModuleBase
    {
        [Command("update")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("Pong!");
        }
    }
}