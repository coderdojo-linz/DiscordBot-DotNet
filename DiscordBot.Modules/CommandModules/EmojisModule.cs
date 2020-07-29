using Discord;
using Discord.Webhook;
using Discord.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Modules
{
    public class EmojisModule : ModuleBase
    {
        internal static Dictionary<string, string> Emojis { get; set; } = new Dictionary<string, string>()
        {
            ["sans"] = "<a:sans:719488632104288286>",
            ["thonk"] = "<:thonk:718853430969368637>",
            ["mindblowing"] = "<:mindblowing:719492411369324584>",
            ["rotatethink"] = "<a:rotatethonk:719491269885427752>",
            ["hyper"] = "<a:hyper:719494595313926244>",
            ["xd"] = "<:xd:719510255402352690>",
            ["rainbowcat"] = "<a:catrainbow1:719513824511656037><a:catrainbow2:719513824553861121>\n<a:catrainbow3:719513824905920603><a:catrainbow4:719513825040400415>",
            ["yes"] = "<a:yes:719496630902063149>",
            ["no"] = "<a:no:719496639471157288>",
            ["calculated"] = "<:calculated:719518217730654218>"
        };

        // If you change the emojis, don't forget to also change the description!!
        [Description(@"
            Verfügbare Emojis:
            - sans (<a:sans:719488632104288286>)
            - thonk (<:thonk:718853430969368637>)
            - mindblowing (<:mindblowing:719492411369324584>)
            - rotatethink (<a:rotatethonk:719491269885427752>)
            - hyper (<a:hyper:719494595313926244>)
            - xd (<:xd:719510255402352690>)
            - rainbowcat (keine Vorschau)
            - yes (<a:yes:719496630902063149>)
            - no (<a:no:719496639471157288>)
            - calculated (<:calculated:719518217730654218>)
        ")]
        [Command("emoji")]
        [Alias("em")]
        public async Task SendEmojiWithName(string name)
        {
            if (!Emojis.TryGetValue(name, out string emoji))
            {
                await ReplyAsync($"Das Emoji {name} wurde nicht gefunden!");
                return;
            }

            var cxt = base.Context;
            var webhook = await ((ITextChannel)cxt.Channel).CreateWebhookAsync(((IGuildUser)cxt.User).Nickname ?? cxt.User.Username);
            var webhookClient = new DiscordWebhookClient(webhook);

            await cxt.Channel.DeleteMessageAsync(base.Context.Message);
            await webhookClient.SendMessageAsync(emoji, avatarUrl: cxt.User.GetAvatarUrl());

            await webhookClient.DeleteWebhookAsync();
        }
    }
}