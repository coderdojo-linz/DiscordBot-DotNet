using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class HelpModule : ModuleBase
    {
        [Command("help")]
        public async Task Help()
        {
            //await ReplyAsync("Pong! Ping: **" +  + "ms**");
        }
    }
}