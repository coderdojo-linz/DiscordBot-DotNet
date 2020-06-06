using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            int ping = base.Context.Client.Latency;
            var embed = new EmbedBuilder();
            embed.WithColor(Color.DarkRed)
                .WithTitle($"Pong :ping_pong: !  Ping: ```{ping} ms```");
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}