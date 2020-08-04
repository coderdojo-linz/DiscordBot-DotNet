﻿using Discord;
using Discord.Commands;

using DiscordBot.Database;
using DiscordBot.Domain.Configuration;
using DiscordBot.Domain.DatabaseModels;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class PollModule : ModuleBase
    {
        private readonly ILogger<FileExplorerModule> _logger;
        private readonly string _prefix;
        private readonly DatabaseContainer<PollInfo> _container;

        public PollModule
        (
            IOptions<DiscordSettings> options, 
            ILogger<FileExplorerModule> logger,
            DatabaseContainer<PollInfo> container
        )
        {
            _logger = logger;
            string prefix = options.Value.CommandPrefix;
            _prefix = prefix;
            _container = container;
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

            var pollInfo = new PollInfo()
            {
                UserId = base.Context.User.Id,
                Title = title,
                IsEnded = false
            };

            for (int i = 0; i < options.Length; i++)
            {
                pollInfo.Options.Add(new OptionEmojiPair
                {
                    Emoji = emojis[i],
                    Option = options[i]
                });
            }

            var builder = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithFooter(footer => footer.Text = $"{_prefix}poll")
                .WithCurrentTimestamp()
                .WithTitle(title);

            StringBuilder desc = new StringBuilder();
            foreach (var emojiPair in pollInfo.Options)
            {
                desc.AppendLine($"{emojiPair.Emoji} - {emojiPair.Option}\n");
            }

            builder.AddField("Reagiere mit einem Emoji um zu voten:", desc.ToString());

            //await base.Context.Message.DeleteAsync();
            IUserMessage message = await base.Context.Channel.SendMessageAsync(embed: builder.Build());
            pollInfo.MessageId = message.Id;
            pollInfo.ChannelId = message.Channel.Id;

            for (int i = 0; i < options.Length; i++)
            {
                await message.AddReactionAsync(new Emoji(emojis[i]));
            }

            //Save database
            await AddEntryToDatabase(pollInfo);
        }

        /// <summary>
        /// 1. LoadDatabase
        /// 2. Append Data
        /// 3. Save Database
        /// </summary>
        /// <returns></returns>
        private Task AddEntryToDatabase(PollInfo info) => _container.Insert(info);
        private async Task<PollInfo> GetPollInfoForMessage(ulong id) => (await _container.Query($"SELECT * FROM db WHERE db.MessageId = {id}")).FirstOrDefault();
        private Task UpdateDb(PollInfo info) => _container.Upsert(info);

        [Command("endpoll")]
        public async Task EndPollAsync(string idOrUrl, [Remainder] string message = "")
        {
            Match idMatch = Regex.Match(idOrUrl, @"^(?:https?://(?:www\.)?discord(?:app)?\.com(?:/channels)/\d+/\d+/)?(\d+)$");
            if (idMatch.Success)
            {
                var id = Convert.ToUInt64(idMatch.Groups[1].Value);
                PollInfo info;
                try
                {
                    info = await GetPollInfoForMessage(id);
                }
                catch
                {
                    await ReplyAsync($"Die angegebene Nachricht ist kein Poll!");
                    return;
                }

                if (info.IsEnded)
                {
                    var errmsg = await ReplyAsync("Der Poll wurde schon beendet!");
                    //await Task.Delay(3000);
                    //await base.Context.Message.DeleteAsync();
                    //await errmsg.DeleteAsync();
                    return;
                }

                ITextChannel channel = (ITextChannel)await base.Context.Guild.GetChannelAsync(info.ChannelId);
                if (channel == null)
                {
                    var errmsg = await ReplyAsync("Der Kanal des Polls wurde nicht gefunden! Vielleicht hast du ihn schon gelöscht!");
                    //await Task.Delay(3000);
                    //await base.Context.Message.DeleteAsync();
                    //await errmsg.DeleteAsync();
                    return;
                }
                IUserMessage msg = (IUserMessage)await channel.GetMessageAsync(id);
                if (msg == null)
                {
                    var errmsg = await ReplyAsync("Der Poll wurde nicht gefunden! Vielleicht hast du ihn schon gelöscht!");
                    //await Task.Delay(3000);
                    //await base.Context.Message.DeleteAsync();
                    //await errmsg.DeleteAsync();
                    return;
                }

                StringBuilder votes = new StringBuilder();
                var voteList = msg.Reactions;
                foreach (OptionEmojiPair option in info.Options)
                {
                    Emoji emoji = new Emoji(option.Emoji);
                    int voteCount = voteList[emoji].ReactionCount - 1;
                    option.Result = voteCount;
                    votes.AppendLine($"{option.Emoji} - {option.Option}: {voteCount} Stimme{(voteCount == 1 ? "" : "n")}\n");
                }
                info.ResultText = message;
                info.IsEnded = true;
                await UpdateDb(info);

                EmbedBuilder emb = new EmbedBuilder()
                    .WithColor(Color.DarkRed)
                    .WithTitle($"Auswertung | {info.Title}")
                    .WithFooter(footer => footer.Text = $"{_prefix}poll")
                    .WithCurrentTimestamp()
                    .AddField("Voteergebnis:", $"{votes}\n{message}");
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
    }
}