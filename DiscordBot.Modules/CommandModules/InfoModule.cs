using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
{
    public class InfoModule : ModuleBase
    {
        private readonly ILogger<PingModule> _logger;

        public InfoModule(ILogger<PingModule> logger)
        {
            _logger = logger;
        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= base.Context.User;
            _logger.LogInformation($"Info über Benutzer {user.Username} angefordert.");

            IGuildUser member = (IGuildUser)user;
            string avatarUrl = user.GetAvatarUrl();
            string[] roleMentions = member.RoleIds
                                          .Where(roleId => roleId != member.Guild.EveryoneRole.Id)
                                          .Select(x => $"<@&{x}>")
                                          .ToArray();

            EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                                            .WithIconUrl(avatarUrl)
                                            .WithName($"{user.Username}#{user.Discriminator}{(user.IsBot ? " [BOT]" : "")}")
                                            ;
            EmbedBuilder embed = new EmbedBuilder()
                                     .WithAuthor(author)
                                     .WithCurrentTimestamp()
                                     .WithThumbnailUrl(avatarUrl)
                                     .WithFooter(x => x.Text = $"ID: {user.Id}")
                                     .WithColor(1219284)
                                     ;

            embed.AddField("Erstellt", user.CreatedAt)
                 .AddField("Beigetreten", member.JoinedAt)
                 .AddField($"Rollen [{roleMentions.Length}]", roleMentions.Length == 0 ? "Keine Rollen" : string.Join(", ", roleMentions))
                 .AddField("Status", GetUserStatusString(user.Status))
                 ;

            await ReplyAsync(embed: embed.Build());
        }

        private string GetUserStatusString(UserStatus status) => status switch
        {
            UserStatus.Online => "Online",
            UserStatus.Idle => "Abwesend",
            UserStatus.AFK => "Abwesend",
            UserStatus.DoNotDisturb => "Nicht stören",
            UserStatus.Invisible => "Offline/Unsichtbar",
            UserStatus.Offline => "Offline/Unsichtbar",
            _ => "Unbekannter Status",
        };

        [Command("channelinfo")]
        public async Task ChannelInfoAsync(IChannel channel = null)
        {
            channel ??= base.Context.Channel;
            _logger.LogInformation($"Info über Kanal {channel.Name} angefordert.");

            EmbedBuilder embed = new EmbedBuilder()
                                     .WithCurrentTimestamp()
                                     .WithFooter(x => x.Text = $"ID: {channel.Id}")
                                     .WithColor(1219284)
                                     ;

            embed.WithTitle("Kanalinformationen")
                 .AddField("Art", GetChannelTypeString(channel))
                 .AddField("Name", $"{channel.Name}")
                 .AddField("Erstellt", $"{channel.CreatedAt}")
                 ;

            if (channel is ITextChannel textChannel)
            {
                embed.AddField("Kategorie", (await textChannel.GetCategoryAsync()).Name);
            }


            if (channel is IVoiceChannel voiceChannel)
            {
                embed.AddField("Kategorie", (await voiceChannel.GetCategoryAsync()).Name);
                if (voiceChannel.UserLimit != null)
                    embed.AddField("Benutzerlimit", $"{(await voiceChannel.GetUsersAsync().ToArrayAsync()).Length}/{voiceChannel.UserLimit}");
            }

            await ReplyAsync(embed: embed.Build());
        }

        private string GetChannelTypeString(IChannel channel) => channel switch
        {
            IMessageChannel _ => "Textkanal",
            IVoiceChannel _ => "Sprachkanal",
            IAudioChannel _ => "Sprachkanal",
            ICategoryChannel _ => "Kategorie",
            _ => "Unbekannt",
        };

        [Group("serverinfo")]
        public class ServerInfoModule : ModuleBase
        {
            private readonly ILogger<PingModule> _logger;

            public ServerInfoModule(ILogger<PingModule> logger)
            {
                _logger = logger;
            }

            [Command("", ignoreExtraArgs: false)]
            public async Task ServerInfoAsync()
            {
                _logger.LogInformation($"Info über Server {base.Context.Guild.Name} angefordert.");

                IGuild guild = base.Context.Guild;
                EmbedBuilder embed = CreateBasicServerEmbed(guild);
                string[] roleMentions = guild.Roles
                                             .Where(role => role.Id != guild.EveryoneRole.Id)
                                             .Select(role => role.Mention)
                                             .ToArray();

                embed.WithTitle("Serverinformationen")
                     .AddField("Owner", (await guild.GetOwnerAsync()).Mention)
                     .AddField("Erstellt", guild.CreatedAt)
                     .AddField("Mitglieder", (await guild.GetUsersAsync()).Count)
                     .AddField("Kanäle", (await guild.GetChannelsAsync()).Count)
                     .AddField($"Rollen [{roleMentions.Length}]", roleMentions.Length == 0 ? "Keine Rollen" : string.Join(", ", roleMentions))
                     ;

                await ReplyAsync(embed: embed.Build());
            }

            [Command("members")]
            public async Task MembersInfoAsync()
            {
                _logger.LogInformation($"Benutzerinfo über Server {base.Context.Guild.Name} angefordert.");

                IGuild guild = base.Context.Guild;
                EmbedBuilder embed = CreateBasicServerEmbed(guild);

                IGuildUser[] members = (await guild.GetUsersAsync()).ToArray();
                embed.WithTitle("Mitglieder")
                     .WithDescription($"{GetMemberString(members)}\n\n{GetMemberStatusString(members)}")
                     ;

                await ReplyAsync(embed: embed.Build());
            }

            [Command("channels")]
            public async Task ChannelsInfoAsync()
            {
                _logger.LogInformation($"Kanalinfo über Server {base.Context.Guild.Name} angefordert.");

                IGuild guild = base.Context.Guild;
                EmbedBuilder embed = CreateBasicServerEmbed(guild);

                embed.WithTitle("Kanäle")
                     .WithDescription(GetChannelString((await guild.GetChannelsAsync()).ToArray()));

                await ReplyAsync(embed: embed.Build());
            }

            [Command("roles")]
            public async Task RolesInfoAsync()
            {
                _logger.LogInformation($"Rolleninfo über Server {base.Context.Guild.Name} angefordert.");

                IGuild guild = base.Context.Guild;
                EmbedBuilder embed = CreateBasicServerEmbed(guild);
                IRole[] roles = guild.Roles.ToArray();
                embed.WithTitle("Rollen")
                     .WithDescription($"**Gesammt:** {roles.Length}\n{GetRoleString(roles)}");

                await ReplyAsync(embed: embed.Build());
            }

            private string GetRoleString(IRole[] roles)
            {
                return string.Join("\n", roles.OrderBy(role => role.Position).Select((role, index) =>
                {
                    return $"**{index})** {role.Mention}{(role.Color.RawValue != 0 ? $" - Farbe: {role.Color}" : "")}";
                }).Reverse());
            }

            private string GetMemberString(IUser[] users)
            {
                int all = users.Length;
                int members = users.Where(user => !user.IsBot).ToArray().Length;
                int bots = users.Where(user => user.IsBot).ToArray().Length;
                return $"**Gesammt:** {all}\n**Personen:** {members}\n**Bots:** {bots}";
            }

            private string GetMemberStatusString(IGuildUser[] members)
            {
                int online = members.Where(x => x.Status == UserStatus.Online).ToArray().Length;
                int afk = members.Where(x => x.Status == UserStatus.AFK || x.Status == UserStatus.Idle).ToArray().Length;
                int dnd = members.Where(x => x.Status == UserStatus.DoNotDisturb).ToArray().Length;
                int invisible = members.Where(x => x.Status == UserStatus.Invisible || x.Status == UserStatus.Offline).ToArray().Length;
                return $":green_circle: {online}  :orange_circle: {afk}  :red_circle: {dnd}  :white_circle: {invisible}";
            }

            private string GetChannelString(IGuildChannel[] channels)
            {
                int categories = channels.Where(channel => channel is ICategoryChannel).ToArray().Length;
                int all = channels.Length - categories;
                int text = channels.Where(channel => channel is ITextChannel).ToArray().Length;
                int voice = channels.Where(channel => channel is IVoiceChannel).ToArray().Length;
                return $"**Gesammt:** {all}\n**Textkanäle:** {text}\n**Sprachkanäle:** {voice}\n(Zusätzlich {categories} Kategorien)";
            }

            private EmbedBuilder CreateBasicServerEmbed(IGuild guild)
            {
                string avatarUrl = guild.IconUrl;

                EmbedBuilder embed = new EmbedBuilder()
                                         .WithAuthor(x =>
                                         {
                                             x.IconUrl = avatarUrl;
                                             x.Name = guild.Name;
                                         })
                                         .WithCurrentTimestamp()
                                         .WithThumbnailUrl(avatarUrl)
                                         .WithFooter(x => x.Text = $"ID: {guild.Id}")
                                         .WithColor(1219284)
                                         ;

                return embed;
            }
        }
    }
}
