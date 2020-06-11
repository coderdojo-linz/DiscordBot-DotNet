using System;
using Discord.Commands;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules
{
    public class PingModule : ModuleBase
    {
        private readonly ILogger<PingModule> logger;

        public PingModule(ILogger<PingModule> logger)
        {
            this.logger = logger;
        }

        [Command("ping")]
        public async Task TestCommandAsync()
        {
          logger.LogInformation("Received ping, answering with poing.");
          await ReplyAsync("Pongedy Ping! Stuff happened!");
        }
    }
}
