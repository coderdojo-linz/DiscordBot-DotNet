using Discord;
using Discord.Commands;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    //-poll "Eins" "Zweites Zwei" "Drittens"
    public class PollModule : ModuleBase<SocketCommandContext>
    {
        public PollModule()
        {
        }

        [Command("poll")]
        public async Task CreatePollAsync(string title, params string[] options)
        {
            var emojis = new[]
            {
                "zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine",
                "keycap_ten",
            }
                .Select(x => GEmojiSharp.Emoji.Get(x))
                .ToList();

            var builder = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithTitle(title);

            var sb = new StringBuilder().AppendLine(" ");
            foreach (var (option, emoji) in options.Zip(emojis, (x, y) => (x, y)))
            {
                sb.AppendLine($"{emoji.Raw}: {option}").AppendLine();
            }

            builder.AddField("Add a reaction to perform your vote", sb);

            var embed = builder.Build();

            var msg = await ReplyAsync(embed: embed);

            foreach (var reaction in emojis.Take(options.Length))
            {
                await msg.AddReactionAsync(new Emoji(reaction.Raw));
            }

            await base.Context.Message.DeleteAsync();
        }
    }
}