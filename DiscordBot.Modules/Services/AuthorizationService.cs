using DiscordBot.Domain.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Modules.Services
{
    public class AuthorizationService
    {
        Permissions _settings;
        public AuthorizationService(IOptions<Permissions> settings)
        {
            _settings = settings.Value;
        }
        public bool IsUserBanned(string username)
        {
            return _settings.banned_users.Contains(username);
        }
        public bool IsUserTrusted(string username)
        {
            return _settings.trusted_users.Contains(username);
        }

        public bool IsUserAdmin(string username)
        {
            return _settings.trusted_users.Contains(username);
        }
    }
}
