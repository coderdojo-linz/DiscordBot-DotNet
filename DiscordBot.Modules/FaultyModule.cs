using System;
using Discord.Commands;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    public class FaultyModule : ModuleBase
    {
        private readonly ILogger<PingModule> logger;

        public FaultyModule(ILogger<PingModule> logger)
        {
            this.logger = logger;
        }

        [Command("log-failure")]
        public async Task LogFailureAsync()
        {
          logger.LogError("Something bad happened...");
          await ReplyAsync("Ups, something bad happened...");
        }

        [Command("throw")]
        public Task ThrowAsync()
        {
            throw new InvalidOperationException("Exception happened...");
        }
    }
}
