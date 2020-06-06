using Discord.Commands;
using DiscordBot.Modules.Services;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class SayModule : ModuleBase<SocketCommandContext>
    {
        private AuthorizationService _authorizationService;

        public SayModule(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        
        [Command("say")]
        public async Task writeanonymous([Remainder] string input)
        {
            if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
                await ReplyAsync(input);
            }
            else
            {
                await ReplyAsync($"Du bist nicht getrustet, {base.Context.User.Mention}");
            }
        }

    }
}