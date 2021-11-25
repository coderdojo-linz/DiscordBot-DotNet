using Discord;
using Discord.Commands;

using DiscordBot.Domain.Abstractions;

using System;
using System.IO;
using System.Threading.Tasks;

using DiscordBot.Modules.Services;
using Discord.Addons.Interactive;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
using System.Linq;

namespace DiscordBot.Modules.CommandModules
{
    [Group("linkShortener")]
    [Alias("short")]
    [Summary("Kürzt einen link auf ``meet.coderdojo.net``")]
    public class LinkShortenerModule : InteractiveBase
    {
        private readonly LinkShortenerService _linkShortenerService;
        private readonly LinkShortenerSettings _settings;

        public LinkShortenerModule(LinkShortenerService linkShortenerService, LinkShortenerSettings settings)
        {
            _linkShortenerService = linkShortenerService;
            _settings = settings;
        }

        [Command("list", RunMode = RunMode.Async)]
        public async Task List([Remainder] string searchterm)
        {
            var items = await _linkShortenerService.GetAllLinksAsync();

            if (!string.IsNullOrWhiteSpace(searchterm))
            {
                items = items.Where(x => LikeOperator.LikeString(x.Id, searchterm, Microsoft.VisualBasic.CompareMethod.Text));
            }

            await PagedReplyAsync(items.Select(x => new EmbedFieldBuilder
            {
                Name = x.Id,
                Value = $"Id: `{x.Id}`\nShortLink: `{x.ShortenedLink}`\nOriginal link: `{x.Url}`"
            }));
        }

        [Command("update")]
        public async Task UpdateLinkAsync(string id, [Remainder] string link)
        {
            var user = base.Context.User;
            if (user is not IGuildUser gu)
            {
                await ReplyAsync("https://flukyfeed.com/wp-content/uploads/2020/07/wait-a-minute.jpg");
                return;
            }

            if (!gu.GuildPermissions.Administrator)
            {
                await ReplyAsync("https://img-9gag-fun.9cache.com/photo/a1rL3vY_700bwp.webp");
                return;
            }

            //base.Context.Guild.user

            await Context.Channel.SendMessageAsync($"Updating {id} to {link}!");
            await _linkShortenerService.UpdateUrlAsync(id, _settings.AccessKey, link);
            await Context.Channel.SendMessageAsync($"Update complete! Ready at http://meet.coderdojo.net/{id}");
        }

        [Command("create")]
        public async Task CreateLinkAsync(string id, [Remainder] string link)
        {
            var user = base.Context.User;
            if (user is not IGuildUser gu)
            {
                await ReplyAsync("https://flukyfeed.com/wp-content/uploads/2020/07/wait-a-minute.jpg");
                return;
            }

            if (!gu.GuildPermissions.Administrator)
            {
                await ReplyAsync("https://img-9gag-fun.9cache.com/photo/a1rL3vY_700bwp.webp");
                return;
            }

            //base.Context.Guild.user

            await Context.Channel.SendMessageAsync($"Creating {id} to {link}!");
            await _linkShortenerService.ShortenUrl(id, _settings.AccessKey, link);
            await Context.Channel.SendMessageAsync($"Create complete! Ready at http://meet.coderdojo.net/{id}");
        }
    }
}