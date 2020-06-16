using Discord.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

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
            var rawEmojis = new[]
            {
                "one",
                "two",
                "three",
                "four",
                "five",
                "six"
            };

            var rotatingNumbers = new[]
            {
                "<a:rotating_numbers_1ms:722399064704024596>",
                "<a:rotating_numbers_10ms:722399064737710121>",
                "<a:rotating_numbers_50ms:722399237413011466>",
            };

            var unicodeEmojis = new List<string>();
            Random randomGen = new Random();

            foreach (var rawEmoji in rawEmojis)
            {
                var emoji = GEmojiSharp.Emoji.Get(rawEmoji);
                unicodeEmojis.Add(emoji.Raw);
            }

            IUserMessage message = null;

            foreach (var rot in rotatingNumbers)
            {
                var text = $"Ein zufälliger Algorithmus würfelt.... :game_die: {rot}";
                if (message == null)
                {
                    message = await Context.Channel.SendMessageAsync(text);
                    await Task.Delay(TimeSpan.FromSeconds(2.5));
                    continue;
                }

                await message.ModifyAsync(x => x.Content = text);
                await Task.Delay(TimeSpan.FromSeconds(2.5));
            }

            int random = randomGen.Next(0, 6);

            await message.ModifyAsync(msg => msg.Content = $"Ein zufälliger Algorithmus würfelt.... :game_die: {unicodeEmojis[random - 1]}\nDie Würfel sind gefallen!");
            logger.LogInformation($"{Context.User} hat die Zahl {random} gewürfelt!");
        }
    }
}