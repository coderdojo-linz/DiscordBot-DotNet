using System.Collections.Generic;
using DiscordBot.Database;
using DiscordBot.Database.Attributes;

namespace DiscordBot.Domain
{
    [ContainerName("polls")]
    public class PollInfo : DatabaseObject
    {
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string Title { get; set; }
        public string ResultText { get; set; }
        public bool IsEnded { get; set; }
        public List<OptionEmojiPair> Options { get; set; } = new List<OptionEmojiPair>();
    }
}