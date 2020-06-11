using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Modules.Services;

namespace DiscordBot.Modules.CommandModules
{
    [Group("mc")]
    [Summary("Manages our minecraft")]
    public class MinecraftModule : ModuleBase<SocketCommandContext>
    {
        private MinecraftService _service;

        public MinecraftModule(MinecraftService service) => _service = service;

        [Command("say")]
        public async Task ListPlayersAsync([Remainder] string text)
        {
            await _service.EnqueueCommandsAsync(new[] { $"say {text}" });
            await base.Context.Channel.SendMessageAsync($"told them '{text}'");
        }
    }
}