using Discord.Commands;
using Microsoft.Extensions.Options;

namespace DiscordBot.Services
{
    public class InjectableCommandService : CommandService
    {
        public InjectableCommandService(IOptions<CommandServiceConfig> config) : base(config.Value)
        {
        }
    }
}
