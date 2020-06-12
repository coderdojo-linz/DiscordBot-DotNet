using DiscordBot.Modules.Utils.ReactionBase;

using System.Threading.Tasks;

namespace DiscordBot.Modules.ReactionModules
{
    //[Reaction("banhammer", "wave", "👋", "poop")]
    [Reaction(ReactionFilter.PassAll)]
    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ReactionAddedAsync()
        {
            await ReplyAsync($"Reaction added! '{base.Context.Reaction.Emote.Name}'");
            return true;
        }

        public override async Task<bool> ReactionRemovedAsync()
        {
            await ReplyAsync($"Reaction Removed! '{base.Context.Reaction.Emote.Name}'");
            return true;
        }

        public override async Task<bool> ReactionClearedAsync()
        {
            await ReplyAsync($"Reactions Cleared!");
            return true;
        }
    }
}