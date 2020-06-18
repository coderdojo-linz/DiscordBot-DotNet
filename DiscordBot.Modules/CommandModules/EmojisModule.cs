using Discord;
using Discord.Commands;
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
            await ReplyAsync("<a:sans:719488632104288286>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("thonk")]
        public async Task Thonk()
        {
            await ReplyAsync("<:thonk:718853430969368637>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("mindblowing")]
        public async Task Mindblowing()
        {
            await ReplyAsync("<:mindblowing:719492411369324584>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("rotatethink")]
        public async Task Rotatethink()
        {
            await ReplyAsync("<a:rotatethonk:719491269885427752>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("hyper")]
        public async Task Hyper()
        {
            await ReplyAsync("<a:hyper:719494595313926244>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("xd")]
        public async Task Xd()
        {
            await ReplyAsync("<:xd:719510255402352690>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("rainbowcat")]
        public async Task Rainbowcat()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<a:catrainbow1:719513824511656037><a:catrainbow2:719513824553861121>");
            sb.AppendLine("<a:catrainbow3:719513824905920603><a:catrainbow4:719513825040400415>");
            await ReplyAsync(sb.ToString());
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("yes")]
        public async Task Yes()
        {
            await ReplyAsync("<a:yes:719496630902063149>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("no")]
        public async Task No()
        {
            await ReplyAsync("<a:no:719496639471157288>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
        [Command("calculated")]
        public async Task Calculated()
        {
            await ReplyAsync("<:calculated:719518217730654218>");
            await base.Context.Channel.DeleteMessageAsync(base.Context.Message);
        }
    }
}