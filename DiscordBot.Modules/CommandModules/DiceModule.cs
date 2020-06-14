using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
{
    public class DiceModule : ModuleBase
    {
        private readonly ILogger<DiceModule> logger;

        public DiceModule(ILogger<DiceModule> logger)
        {
            this.logger = logger;
        }

        [Command("dice")]
        public async Task Dice()
        {
            var emojis = new[]
            {
                "one",
                "two",
                "three",
                "four",
                "five",
                "six"
            };
            var unicodeEmojis = new List<string>();
            Random randomGen = new Random();
            for (int i = 1; i < 6; i++)
            {
                unicodeEmojis.Add(GEmojiSharp.Emoji.Get(emojis[i]).Raw);
            }                
            int random = randomGen.Next(1, 6);
            Discord.IUserMessage msg = await Context.Channel.SendMessageAsync($":game_die: Du hast die Zahl {unicodeEmojis[random-1]} gewürfelt!");
            for (int i = 1; i < 5; i++)
            {
                random = randomGen.Next(1, 6);
                await msg.ModifyAsync(msg => msg.Content = $":game_die: Du hast die Zahl {unicodeEmojis[random-1]} gewürfelt!");
                await Task.Delay(500);
            }
            logger.LogInformation($"{Context.User} hat die Zahl {random + 1} gewürfelt!");
        }
    }
}
