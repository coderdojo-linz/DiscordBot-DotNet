using DiscordBot.Database;
using DiscordBot.Database.Attributes;
using System.Collections.Generic;

namespace DiscordBot.Domain.DatabaseModels
{
    [ContainerName("emojiwebhooks")]
    public class EmojiWebhook : DatabaseObject
    {
        public ulong UserId { get; set; }
        public Dictionary<ulong, ulong> ChannelsAndWebhooks { get; set; }
    }
}