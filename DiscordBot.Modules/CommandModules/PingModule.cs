using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
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
          await ReplyAsync("Pongedy Ping!!");
        }
    }
}
