using Discord;
using Discord.Commands;

using DiscordBot.Domain.Abstractions;

using System;
using System.IO;
using System.Threading.Tasks;

using DiscordBot.Modules.Services;
using Discord.Addons.Interactive;
using System.Collections.Generic;

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
        public async Task List()
        {
            //  var embed = new EmbedBuilder();

            var pages = new List<EmbedFieldBuilder>();

            var results = await _linkShortenerService.GetAllLinksAsync();
            foreach (var item in results)
            {
                pages.Add(new EmbedFieldBuilder()
                {
                    Name = item.Id,
                    Value = $"Id: `{item.Id}`\nShortLink: `{item.ShortenedLink}`\nOriginal link: `{item.Url}`"
                });

                //embed.AddField(x =>
                //{
                //    x.Name = item.Id;
                //    x.Value = $"ShortLink: {item.ShortenedLink}\nOriginal link: {item.Url}";
                //});
            }

            //var pages = new[] { "Page 1", "Page 2", "Page 3", "aaaaaa", "Page 5" };
            await PagedReplyAsync(pages);
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