using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

namespace DiscordBot.Services.Base
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

    public abstract class ReactionModuleBase
    {
        public ReactionContext Context { get; set; }

        public abstract Task<bool> ExecuteAsync();
    }

    public class ReactionTestModule : ReactionModuleBase
    {
        public override async Task<bool> ExecuteAsync()
        {
            await base.Context.MessageChannel.SendMessageAsync($"Cool Reaction bro! '{base.Context.Reaction.Emote.Name}'");
            return true;
        }
    }
}