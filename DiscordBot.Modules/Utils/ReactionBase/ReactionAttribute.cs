using System;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public class ReactionAttribute : Attribute
    {
        public string[] ReactionName { get; }

        public ReactionAttribute(params string[] reactionName)
        {
            ReactionName = reactionName;
        }
    }
}