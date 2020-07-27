﻿using DiscordBot.Modules.Utils.ReactionBase;

using System.Threading.Tasks;

namespace DiscordBot.Modules.ReactionModules
{
    [Reaction("banhammer", "dojo", "wave")]
    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ReactionAddedAsync()
        {
            if (!(base.Context.Channel.Name?.Contains("bot-spam") ?? false))
            {
                return false;
            }

            var emote = base.Context.Reaction.Emote switch
            {
                // For this to work, the bot has to be in the same server, the emote is from
                // Because of this, banhammer wont work because the bot is not in the same server
                Discord.Emote em => $"<:{em.Name}:{em.Id}>",
                Discord.Emoji emoji => $"{emoji.Name}",
                _ => ""
            };

            await ReplyAsync($"Reaktion hinzugefügt! '{emote}'");

            return true;
        }

        public override async Task<bool> ReactionRemovedAsync()
        {
            await ReplyAsync($"Reaktion entfernt! '{base.Context.Reaction.Emote.Name}'");
            // Application insights also works here:
            // throw new AggregateException("I don't like this");
            return true;
        }

        public override async Task<bool> ReactionClearedAsync()
        {
            await ReplyAsync($"Reaktionen gelöscht!");
            return true;
        }
    }
}