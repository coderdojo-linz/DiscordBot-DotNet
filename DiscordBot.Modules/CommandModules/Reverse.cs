using Discord.Commands;
using System;
using System.Threading.Tasks;
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
