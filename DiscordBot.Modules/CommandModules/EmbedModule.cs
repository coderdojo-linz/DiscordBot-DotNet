using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Modules.Services;

namespace DiscordBot.Modules.CommandModules
{
    public class EmbedModule : ModuleBase
    {
        private readonly InteractivityService _interactivity;

        public EmbedModule(InteractivityService interactivity)
        {
            _interactivity = interactivity;
        }

        [Command("embed")]
        public async Task CreateEmbedAsync(IChannel channel = null)
        {
            channel ??= base.Context.Channel;

            await base.Context.Message.DeleteAsync();

            string title = await Input("Was ist dein Titel?");
            if (title == null) return;
            string content = await Input("Was ist dein Inhalt?");
            if (content == null) return;
            string color = await Input("Was ist deine Farbe?");
            if (color == null) return;

            await ((IMessageChannel)channel).SendMessageAsync(embed: new EmbedBuilder()
                                                                         .WithTitle(title)
                                                                         .WithDescription(content)
                                                                         .WithColor(color[0] == '#'
                                                                                        ? uint.Parse(color.Substring(1), System.Globalization.NumberStyles.HexNumber)
                                                                                        : uint.Parse(color, System.Globalization.NumberStyles.HexNumber))
                                                                         .Build());
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
    }
}
