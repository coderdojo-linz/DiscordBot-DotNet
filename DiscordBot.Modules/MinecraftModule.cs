using Discord.Commands;
using LibMCRcon.RCon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("mc")]
    [Summary("Manages our minecraft")]
    public class MinecraftModule : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        public async Task ListPlayersAsync()
        {
            var rcon = LibMCRcon.RCon.MCHelper.ActivateRcon("XXX", 25575, "XXX");
            var errsb = new StringBuilder();
            var players = MCHelper.LoadPlayers(rcon, errsb);


          

            rcon.StopComms();
            await base.Context.Channel.SendMessageAsync(string.Join("\n", players));
        }
    }
}
