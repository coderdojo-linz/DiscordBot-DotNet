using Discord.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            var msg = await Context.Channel.SendMessageAsync($"Ein zufälliger Algorithmus würfelt.... :game_die: <a:rotating:722023788736151623>");

            await Task.Delay(TimeSpan.FromSeconds(5));
            await msg.ModifyAsync(msg => msg.Content = $":game_die: Du hast die Zahl {unicodeEmojis[random - 1]} gewürfelt!");
            logger.LogInformation($"{Context.User} hat die Zahl {random + 1} gewürfelt!");
        }
    }
}