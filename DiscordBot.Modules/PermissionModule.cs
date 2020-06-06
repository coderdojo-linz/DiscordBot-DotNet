using Discord.Commands;
using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class PermissionsModule : ModuleBase
    {
        private AuthorizationService _authorizationService;

        public PermissionsModule(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [Command("isTrusted")]
        public async Task IsTrusted(string user)
        {
            if (_authorizationService.IsUserTrusted(user))
            {
                await ReplyAsync($"Der Benutzer {user} ist getrustet");
            }
            else
            {
                await ReplyAsync($"Der Benutzer {user} ist nicht getrustet");
            }
        }
        [Command("isBanned")]
        public async Task IsBanned(string user)
        {
            if (_authorizationService.IsUserBanned(user))
            {
                await ReplyAsync($"Der Benutzer {user} ist gebannt");
            }
            else
            {
                await ReplyAsync($"Der Benutzer {user} ist nicht gebannt");
            }
        }
    }
}