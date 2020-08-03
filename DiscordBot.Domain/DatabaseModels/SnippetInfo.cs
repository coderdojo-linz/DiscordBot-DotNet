using DiscordBot.Database;
using DiscordBot.Database.Attributes;

namespace DiscordBot.Domain.DatabaseModels
{
    [ContainerName("codesnippets")]
    public class SnippetInfo : DatabaseObject
    {
        public ulong UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
