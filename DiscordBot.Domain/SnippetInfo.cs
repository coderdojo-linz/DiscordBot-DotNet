﻿using DiscordBot.Database;
namespace DiscordBot.Domain
{
    public class SnippetInfo : DatabaseObject
    {
        public ulong UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
