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



        public ulong MessageId => _cachedUserMessage.Id;
        public IUserMessage Message => _cachedUserMessage.HasValue ? _cachedUserMessage.Value : null;
        public SocketReaction Reaction => _reaction;
        public ISocketMessageChannel Channel => _messageChannel;
    }
}