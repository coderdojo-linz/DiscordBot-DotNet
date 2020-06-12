using System;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public class ReactionAttribute : Attribute
    {
        public ReactionFilter FilterMode { get; }
        public string[] ReactionNames { get; }

        public ReactionAttribute(params string[] reactionName) : this(ReactionFilter.Select, reactionName)
        {
        }

        public ReactionAttribute(ReactionFilter selection, params string[] reactionName)
        {
            FilterMode = selection;
            ReactionNames = reactionName ?? Array.Empty<string>();
        }
    }
}