using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Modules.Services;

namespace DiscordBot.Modules.CommandModules
{
    public class CheckModule : ModuleBase
    {
        private readonly InteractivityService _interactivity;

        public CheckModule(InteractivityService interactivity)
        {
            _interactivity = interactivity;
        }

        [Command("reactions")]
        public async Task CheckEmbedAsync(IChannel channel = null)
        {
            channel ??= base.Context.Channel;

            await base.Context.Message.DeleteAsync();

            string question = await Input("Was ist deine Frage?");
            if (question == null) return;

            var q = await ((IMessageChannel)channel).SendMessageAsync(embed: new EmbedBuilder()
                                                                                 .WithTitle("Frage:")
                                                                                 .WithDescription($"{question}\n\nReagiere mit <a:yes:719496630902063149> oder <a:no:719496639471157288>")
                                                                                 .WithColor(0x129AD4)
                                                                                 .Build());

            await q.AddReactionAsync(new Emoji("a:yes:719496630902063149"));
            await q.AddReactionAsync(new Emoji("a:no:719496639471157288"));
            await Task.Delay(100); // HACK: I don't know why this is necessary, but
                                   //       maybe that's because AddReactionAsync()
                                   //       doesn't wait until the reaction is added,
                                   //       but until the request is sent.

            var reaction = await React(q);
            if (reaction == null) return;

            await q.ModifyAsync(m =>
            {
                m.Embed = new EmbedBuilder()
                              .WithTitle("Frage:")
                              .WithDescription($"{question}\n\nErgebnis: {reaction.Emote}")
                              .WithColor(0x129AD4)
                              .Build();
            });
        }

        public async Task<string> Input(string question)
        {
            var q = await ReplyAsync(question);
            var a = (await _interactivity.AwaitMessagesAsync(base.Context.Channel, message => message.Author == base.Context.User, 1)).FirstOrDefault();
            await q.DeleteAsync();
            if (a != null)
                await a.DeleteAsync();
            if (a == null)
            {
                var cancel = await ReplyAsync("Abgebrochen");
                await Task.Delay(1000);
                await cancel.DeleteAsync();
                return null;
            }
            return a.Content;
        }

        public async Task<IReaction> React(IMessage q)
        {
            var a = (await _interactivity.AwaitReactionsAsync(q,

                reaction =>
                {
                    if (reaction.Emote is Emote em)
                        return em.Id == 719496630902063149 || em.Id == 719496639471157288;
                    return false;
                },
                1)).FirstOrDefault();
            if (a == null)
            {
                var cancel = await ReplyAsync("Abgebrochen");
                await Task.Delay(1000);
                await q.DeleteAsync();
                await cancel.DeleteAsync();
                return null;
            }
            return a;
        }
    }
}
