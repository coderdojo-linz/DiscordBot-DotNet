using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Modules.Services
{
    public class InteractivityService
    {
        private readonly DiscordSocketClient _client;

        public InteractivityService(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task<IMessage[]> AwaitMessagesAsync(IChannel channel, MessageFilter messageFilter = null, int maxMessages = int.MaxValue, int maxTime = 10000)
        {
            if (channel is null)
            {
                throw new System.ArgumentNullException(nameof(channel));
            }

            TaskCompletionSource<IMessage[]> promise = new TaskCompletionSource<IMessage[]>();

            Task.Run(() =>
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                List<IMessage> result = new List<IMessage>();
                Task MessageHandler(IMessage message)
                {
                    if (messageFilter != null && !messageFilter(message)) return Task.CompletedTask;
                    result.Add(message);
                    if (result.Count >= maxMessages)
                    {
                        cts.Cancel();
                        return Task.CompletedTask;
                    }
                    return Task.CompletedTask;
                }
                _client.MessageReceived += MessageHandler;
                _ = Task.Run(async () =>
                {
                    try
                    { 
                        await Task.Delay(maxTime, cts.Token);
                    }
                    catch { }
                    _client.MessageReceived -= MessageHandler;
                    promise.TrySetResult(result.ToArray());
                });
            });

            return promise.Task;
        }

        public Task<IReaction[]> AwaitReactionsAsync(IMessage message, ReactionFilter reactionFilter = null, int maxReactions = int.MaxValue, int maxTime = 10000)
        { 
            TaskCompletionSource<IReaction[]> promise = new TaskCompletionSource<IReaction[]>();

            Task.Run(() =>
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                List<IReaction> result = new List<IReaction>();
                Task ReactionHandler(Cacheable<IUserMessage, ulong> m, IChannel c, SocketReaction reaction)
                {
                    if (message.Id != m.Id) return Task.CompletedTask;
                    if (reactionFilter != null && !reactionFilter(reaction)) return Task.CompletedTask;
                    result.Add(reaction);
                    if (result.Count >= maxReactions)
                    {
                        cts.Cancel();
                        return Task.CompletedTask;
                    }
                    return Task.CompletedTask;
                }
                _client.ReactionAdded += ReactionHandler;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(maxTime, cts.Token);
                    }
                    catch { }
                    _client.ReactionAdded -= ReactionHandler;
                    promise.TrySetResult(result.ToArray());
                });
            });

            return promise.Task;
        }

        public delegate bool MessageFilter(IMessage message);
        public delegate bool ReactionFilter(SocketReaction reactionEmoji);
    }
}
