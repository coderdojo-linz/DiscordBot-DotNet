using Discord;
using Discord.Commands;
using Discord.Webhook;


using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("emoji")]
    [Alias("em")]
    public class EmojisModule : ModuleBase
    {
        [Command("sans")]
        public async Task Sans()
        {
            await SendEmojiWithName("sans");
        }
        [Command("thonk")]
        public async Task Thonk()
        {
            await SendEmojiWithName("thonk");
        }
        [Command("mindblowing")]
        public async Task Mindblowing()
        {
            await SendEmojiWithName("mindblowing");
        }
        [Command("rotatethink")]
        public async Task Rotatethink()
        {
            await SendEmojiWithName("rotatethink");
        }
        [Command("hyper")]
        public async Task Hyper()
        {
            await SendEmojiWithName("hyper");
        }
        [Command("xd")]
        public async Task Xd()
        {
            await SendEmojiWithName("xd");
        }
        [Command("rainbowcat")]
        public async Task Rainbowcat()
        {
            await SendEmojiWithName("rainbowcat");
        }
        [Command("yes")]
        public async Task Yes()
        {
            await SendEmojiWithName("yes");
        }
        [Command("no")]
        public async Task No()
        {
            await SendEmojiWithName("no");
        }
        [Command("calculated")]
        public async Task Calculated()
        {
            await SendEmojiWithName("calculated");
        }

        private async Task SendEmojiWithName(string name)
        {
            string emoji = name switch
            {
                "sans" => "<a:sans:719488632104288286>",
                "thonk" => "<:thonk:718853430969368637>",
                "mindblowing" => "<:mindblowing:719492411369324584>",
                "rotatethink" => "<a:rotatethonk:719491269885427752>",
                "hyper" => "<a:hyper:719494595313926244>",
                "xd" => "<:xd:719510255402352690>",
                "rainbowcat" => "<a:catrainbow1:719513824511656037><a:catrainbow2:719513824553861121>\n<a:catrainbow3:719513824905920603><a:catrainbow4:719513825040400415>",
                "yes" => "<a:yes:719496630902063149>",
                "no" => "<a:no:719496639471157288>",
                "calculated" => "<:calculated:719518217730654218>",
                _ => "unknown"
            };

            var cxt = base.Context;
            var webhook = await ((ITextChannel)cxt.Channel).CreateWebhookAsync(((IGuildUser)cxt.User).Nickname ?? cxt.User.Username);
            var webhookClient = new DiscordWebhookClient(webhook);

            await cxt.Channel.DeleteMessageAsync(base.Context.Message);
            await webhookClient.SendMessageAsync(emoji, avatarUrl: cxt.User.GetAvatarUrl());

            await webhookClient.DeleteWebhookAsync();
        }
    }
}