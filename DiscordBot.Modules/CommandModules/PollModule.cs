using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task CreatePollAsync(string title, params string[] options)
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
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine",
                "keycap_ten"
            }
                .Select(emoji => GEmojiSharp.Emoji.Get(emoji).Raw)
                .ToList();

            var builder = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithFooter(footer => footer.Text = $"{_prefix}poll")
                .WithCurrentTimestamp()
                .WithTitle(title);

            string desc = "";
            for (int i = 0; i < options.Length; i++)
            {
                desc += $"{emojis[i]} - {options[i]}\n";
            }
            builder.AddField("Reagiere mit einem Emoji um zu voten:", desc);

            //await base.Context.Message.DeleteAsync();
            IUserMessage message = await base.Context.Channel.SendMessageAsync(embed: builder.Build());
            for (int i = 0; i < options.Length; i++)
            {
                await message.AddReactionAsync(new Emoji(emojis[i]));
            }
        }

        [Command("endpoll")]
        public async Task EndPollAsync(string idOrUrl, [Remainder] string message = "")
        {
            Match idMatch = Regex.Match(idOrUrl, @"^(?:https?://(?:www\.)?discord(?:app)?\.com(?:/channels)/\d+/\d+/)?(\d+)$");
            if (idMatch.Success)
            {
                IUserMessage msg = (IUserMessage)await base.Context.Channel.GetMessageAsync(Convert.ToUInt64(idMatch.Groups[1].Value));
                if (msg == null)
                {
                    var errmsg = await ReplyAsync("Der Poll wurde nicht gefunden! Stelle sicher, dass sich der Poll in dem aktuellen Kanal befindet!");
                    //await Task.Delay(3000);
                    //await base.Context.Message.DeleteAsync();
                    //await errmsg.DeleteAsync();
                    return;
                }
                if (!CheckPollMsg(msg))
                {
                    var errmsg = await ReplyAsync("Die angegebene Nachricht ist kein Poll!");
                    //await Task.Delay(3000);
                    //await base.Context.Message.DeleteAsync();
                    //await errmsg.DeleteAsync();
                    return;
                }
                var embed = msg.Embeds.First();

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
                //await base.Context.Message.DeleteAsync();
                await msg.ModifyAsync(msg => msg.Embed = emb.Build());
            }
            else
            {
                var err = await ReplyAsync("Bitte gib eine gültige ID oder Url an!");
                //await Task.Delay(3000);
                //await base.Context.Message.DeleteAsync();
                //await err.DeleteAsync();
            }
        }

        private bool CheckPollMsg(IUserMessage msg)
        {
            //Console.WriteLine($"`{msg.Embeds.First().Fields.Count()}Desc");
            if ((msg.Content != "") ||
                (msg.Embeds.Count != 1) ||
                (msg.Embeds.First().Title == "") ||
                (msg.Embeds.First().Footer.ToString() != $"{_prefix}poll") ||
                (msg.Embeds.First().Timestamp == null) ||
                (msg.Embeds.First().Description != null) ||
                (msg.Embeds.First().Fields.Count() != 1) ||
                (msg.Embeds.First().Fields.First().Value == "")) { return false; }
            return true;
        }
    }
}
