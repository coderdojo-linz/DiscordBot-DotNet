using DiscordBot.Modules.Utils.ReactionBase;

using System.Threading.Tasks;

namespace DiscordBot.Modules.ReactionModules
{
    //"wave": 👋
    //"poop": 💩
    [Reaction("banhammer", "dojo", "👋", "💩")]
    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ReactionAddedAsync()
        {
            var emote = base.Context.Reaction.Emote switch
            {
                // For this to work, the bot has to be in the same server, the emote is from
                // Because of this, banhammer wont work because the bot is not in the same server
                Discord.Emote em => $"<:{em.Name}:{em.Id}>",
                Discord.Emoji emoji => $"{emoji.Name}",
                _ => ""
            };

            await ReplyAsync($"Reaction added! '{emote}'");

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