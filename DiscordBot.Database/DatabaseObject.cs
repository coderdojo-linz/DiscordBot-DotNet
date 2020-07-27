using System;
using Newtonsoft.Json;

namespace DiscordBot.Database
{
    public abstract class DatabaseObject
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [JsonProperty("partkey")]
        public string PartKey { get; set; }
    }
}