using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using DiscordBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordBot.Modules.CommandModules
{
    public class PollModule : ModuleBase
    {
        string _prefix;
        public PollModule(IOptions<DiscordSettings> options)
        {
            string prefix = options.Value.CommandPrefix;
            _prefix = prefix;
        }

        [Command("poll")]
        public async Task CreatePoll(string title, params string[] options)
        {
            if (options.Length > 10)
            {
                var msg = await ReplyAsync("Das Maximum an 10 Optionen von `poll` wurde überschritten!");
                await Task.Delay(3000);
                await base.Context.Message.DeleteAsync();
                await msg.DeleteAsync();
                return;
            }
            var emojis = new[]
            {
                "one", "two", "three",
                "four", "five", "six", "seven",
                "eight", "nine", "keycap_ten"
            };
            var unicodeEmojis = new List<string>();
            for (int i = 0; i < options.Length; i++)
            {
                unicodeEmojis.Add(GEmojiSharp.Emoji.Get(emojis[i]).Raw);
            }

            var builder = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithFooter(footer => footer.Text = $"{_prefix}poll")
                .WithCurrentTimestamp()
                .WithTitle(title);

            string desc = "";
            for (int i = 0; i < options.Length; i++)
            {
                desc += $"{unicodeEmojis[i]} - {options[i]}\n";
            }
            builder.AddField("Reagiere mit einem Emoji um zu voten:", desc);

            await base.Context.Message.DeleteAsync();
            IUserMessage message = await base.Context.Channel.SendMessageAsync(embed: builder.Build());
            foreach (var emoji in unicodeEmojis)
            {
                await message.AddReactionAsync(new Emoji(emoji));
            }
        }

        [Command("endpoll")]
        public async Task EndPoll(string idOrUrl, [Remainder] string message = "")
        {
            Match idMatch = Regex.Match(idOrUrl, @"^(?:https?://(?:www\.)?discord(?:app)?\.com(?:/channels)/\d+/\d+/)?(\d+)$");
            if (idMatch.Success)
            {
                IUserMessage msg = (IUserMessage)await base.Context.Channel.GetMessageAsync(Convert.ToUInt64(idMatch.Groups[1].Value));
                if (msg == null)
                {
                    var errmsg = await ReplyAsync("Der Poll wurde nicht gefunden! Stelle sicher, dass sich der Poll in dem aktuellen Kanal befindet!");
                    await Task.Delay(3000);
                    await base.Context.Message.DeleteAsync();
                    await errmsg.DeleteAsync();
                    return;
                }
                var embed = msg.Embeds.ToArray()[0];

                string[] availableOptions = embed.Fields[0].Value.Split("\n");
                string votes = "";
                var voteList = msg.Reactions;
                for (int i = 0; i < availableOptions.Length; i++)
                {
                    Emoji emoji = new Emoji(Regex.Match(availableOptions[i], @"^([^ -]+) - ").Groups[1].Value);
                    string option = availableOptions[i];
                    int voteCount = voteList[emoji].ReactionCount - 1;
                    votes += $"{availableOptions[i]}: {voteCount} Stimme{(voteCount == 1 ? "" : "n")}\n";
                }
                EmbedBuilder emb = new EmbedBuilder()
                    .WithColor(Color.DarkRed)
                    .WithTitle($"Auswertung | {embed.Title}")
                    .WithFooter(footer => footer.Text = $"{_prefix}poll")
                    .WithCurrentTimestamp()
                    .WithDescription($"Voteergebnis:\n{votes}\n{message}");
                await base.Context.Message.DeleteAsync();
                await msg.ModifyAsync(msg => msg.Embed = emb.Build());
            }
            else
            {
                await base.Context.Message.DeleteAsync();
                var err = await ReplyAsync("Please specify a valid id or url!");
                await Task.Delay(3000);
                await err.DeleteAsync();
            }
        }
    }
}
