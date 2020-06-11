using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
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
          logger.LogError("Something bad happened....");
          await ReplyAsync("Ups, something bad happened....");
        }

        [Command("throw")]
        public Task ThrowAsync()
        {
            throw new InvalidOperationException("Exception happened...");
        }
    }
}
