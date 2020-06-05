using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    //https://api.thecatapi.com/v1/images/search?breed_ids=abys
    public class PingModule : ModuleBase
    {
        [Command("ping")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("ping2")]
        public async Task Ping2() => await TestCommandAsync();

        [Command("hello")]
        public async Task HelloWorld()
        {
            await ReplyAsync("World!");
        }
    }
}