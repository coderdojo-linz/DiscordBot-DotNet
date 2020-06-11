using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public class ReactionContext
    {
        private readonly Cacheable<IUserMessage, ulong> _cachedUserMessage;
        private readonly ISocketMessageChannel _messageChannel;
        private readonly SocketReaction _reaction;

        public ReactionContext(Cacheable<IUserMessage, ulong> cachedUserMessage, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            _cachedUserMessage = cachedUserMessage;
            _messageChannel = messageChannel;
            _reaction = reaction;
        }

        public IUserMessage UserMessage => _cachedUserMessage.HasValue ? _cachedUserMessage.Value : null;
        public SocketReaction Reaction => _reaction;
        public ISocketMessageChannel MessageChannel => _messageChannel;
    }

    public class ReactionAttribute : Attribute
    {
        public string[] ReactionName { get; }

        public ReactionAttribute(params string[] reactionName)
        {
            ReactionName = reactionName;
        }
    }

    public abstract class ReactionModuleBase
    {
        public ReactionContext Context { get; set; }

        public abstract Task<bool> ExecuteAsync();
    }
}