using DiscordBot.Modules.Utils.ReactionBase;

using System.Threading.Tasks;

namespace DiscordBot.Modules.ReactionModules
{
    [Reaction("banhammer", "wave", "👋", "poop")]
    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ExecuteAsync()
        {
            await ReplyAsync($"Cool Reaction bro! '{base.Context.Reaction.Emote.Name}'");
            return true;
        }
    }
}