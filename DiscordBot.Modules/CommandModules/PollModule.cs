using Discord;
using Discord.Commands;

using DiscordBot.Domain.Configuration;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiscordBot.Modules.CommandModules
{



    public class PollInfo
    {
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public List<OptionEmojiPair> Options { get; set; } = new List<OptionEmojiPair>();
    }

    public class OptionEmojiPair
    {
        public string Option { get; set; }
        public string Emoji { get; set; }
    }

    public class PollModule : ModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileExplorerModule> _logger;
        private string _prefix;

        public PollModule
        (
            IOptions<DiscordSettings> options, 
            IConfiguration configuration,
            ILogger<FileExplorerModule> logger 
        )
        {
            _configuration = configuration;
            _logger = logger;
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

            var pollInfo = new PollInfo()
            {
                UserId = base.Context.User.Id
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
        public async Task AddEntryToDatabase(PollInfo pollInfo)
        {
            var db = await ReadDatabase();
            db.Add(pollInfo);

            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "PollModule"));
            var serialized = JsonConvert.SerializeObject(db);
            await File.WriteAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "PollDb.json"), serialized);
        }

        public async Task<List<PollInfo>> ReadDatabase()
        {
            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return null;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "PollModule"));
            if (!File.Exists(Path.Combine(pollModuleDirectory.FullName, "PollDb.json")))
            {
                return new List<PollInfo>();
            }

            var content = await File.ReadAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "PollDb.json"));
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<PollInfo>>(content);

        }

        public async Task<PollInfo> GetPollInfoForMessage(ulong messageId)
        {
            var db = await ReadDatabase();
            return db.Where(x => x.MessageId == messageId).FirstOrDefault();
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

                string[] availableOptions = Regex.Replace(embed.Fields[0].Value, @"\n+", "\n").Split("\n");
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
            if (msg.Content != "")
            {
                return false;
            }

            if (msg.Embeds.Count != 1)
            {
                return false;
            }

            var emb = msg.Embeds.First();

            if (emb.Title == "")
            {
                return false;
            }

            if (emb.Footer.ToString() != $"{_prefix}poll")
            {
                return false;
            }

            if (emb.Timestamp == null)
            {
                return false;
            }

            if (emb.Description != null)
            {
                return false;
            }

            var fields = emb.Fields.Take(2).ToList();

            if (fields.Count != 1)
            {
                return false;
            }

            if (fields[0].Value == "")
            {
                return false;
            }

            return true;
        }
    }
}