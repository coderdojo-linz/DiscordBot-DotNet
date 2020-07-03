using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
{
    public class ReactionModule : ModuleBase
    {
        private readonly ILogger<PingModule> logger;

        public ReactionModule(ILogger<PingModule> logger)
        {
            this.logger = logger;
        }

        [Command("react")]
        [Summary("Der Bot reagiert mit dem angegebenen Emoji auf die angegebene Nachricht.")]
        public async Task AddReaction(string name, string URL)
        {
            await ReactWithEmoji(name, URL);
        }
        private async Task ReactWithEmoji(string name, string URL)
        {
            string emojilist = name switch
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
            if (emojilist == "unknown")
            {
                await ReplyAsync($"Konnte das Emoji {name} nicht finden!");
            }
            else if (emojilist == "<a:catrainbow1:719513824511656037><a:catrainbow2:719513824553861121>\n<a:catrainbow3:719513824905920603><a:catrainbow4:719513825040400415>")
            {
                await ReplyAsync("Die Rainbowkatze kann nicht in einer Reaktion verwendet werden!");
            }
            else
            {
                var emoji = Emote.Parse(emojilist);
                Match idMatch = Regex.Match(URL, @"^(?:https?://(?:www\.)?discord(?:app)?\.com(?:/channels)/\d+/\d+/)?(\d+)$");
                if (idMatch.Success)
                {
                    IUserMessage msg = (IUserMessage)await base.Context.Channel.GetMessageAsync(Convert.ToUInt64(idMatch.Groups[1].Value));
                    await msg.AddReactionAsync(emoji);
                    await base.Context.Message.DeleteAsync();
                }
            }
        }
    }
}
