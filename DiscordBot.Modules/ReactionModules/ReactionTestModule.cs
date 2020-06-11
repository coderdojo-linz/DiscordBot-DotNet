using System.Threading.Tasks;
using DiscordBot.Modules.Utils.ReactionBase;

namespace DiscordBot.Modules.ReactionModules
{
    [Reaction("banhammer", "wave", "👋")]
    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ExecuteAsync()
        {
            await ReplyAsync($"Cool Reaction bro! '{base.Context.Reaction.Emote.Name}'");
            return true;
        }
    }
}