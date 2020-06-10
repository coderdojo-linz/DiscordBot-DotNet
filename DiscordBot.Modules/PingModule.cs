using System;
using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class PingModule : ModuleBase
    {
        [Command("ping")]
        public async Task TestCommandAsync()
        {
          await ReplyAsync("Pongedy Ping! Stuff happened!");
        }
    }
}
