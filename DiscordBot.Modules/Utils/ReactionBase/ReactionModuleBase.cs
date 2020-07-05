using Discord;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Utils.ReactionBase
{
    public abstract class ReactionModuleBase
    {
        public ReactionContext Context { get; set; }

        public virtual Task<bool> ExecuteAsync() => Context.ReactionType switch
        {
            ReactionType.Added => ReactionAddedAsync(),
            ReactionType.Removed => ReactionRemovedAsync(),
            ReactionType.Cleared => ReactionClearedAsync(),
            ReactionType.Default => Task.FromResult(false),
            _ => Task.FromResult(false)
        };

        public virtual Task<bool> ReactionAddedAsync()
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> ReactionRemovedAsync()
        {
            return Task.FromResult(false);
        }

        public virtual Task<bool> ReactionClearedAsync()
        {
            return Task.FromResult(false);
        }

        /// <summary>Sends a message to the source channel.</summary>
        /// <param name="message">
        /// Contents of the message; optional only if <paramref name="embed" /> is specified.
        /// </param>
        /// <param name="isTTS">Specifies if Discord should read this <paramref name="message" /> aloud using text-to-speech.</param>
        /// <param name="embed">An embed to be displayed alongside the <paramref name="message" />.</param>
        protected virtual async Task<IUserMessage> ReplyAsync(
            string message = null,
            bool isTTS = false,
            Embed embed = null,
            RequestOptions options = null)
        {
            return await this.Context.Channel.SendMessageAsync(message, isTTS, embed, options).ConfigureAwait(false);
        }

        //    var msg = cachedMessage.HasValue ? cachedMessage.Value : await socketMessageChannel.GetMessageAsync(cachedMessage.Id);

        public async Task<IMessage> GetMessageAsync()
        {
            return Context.Message ?? await Context.Channel.GetMessageAsync(Context.MessageId);
        }
    }
}