using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using System.Linq;

namespace DiscordBot.Modules
{
    public class Reverse : ModuleBase
    {  
        [Command("reverse")]
        public async Task reverse([Remainder]string Input)
        {
            var reversedWord = Input.ToCharArray().Reverse().ToArray();

            await ReplyAsync(new string(reversedWord));
            
        }      
    }
}
