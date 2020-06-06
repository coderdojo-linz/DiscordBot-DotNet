using Discord;
using Discord.Commands;
using DiscordBot.Modules.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CustomStatusModule : ModuleBase<SocketCommandContext>
    {
        static private Timer _timer;
        static private int _statusIndex = 0;
        private AuthorizationService _authorizationService;
        static public int _abort = 1;

        static List<string> _statusList = new List<string>() { "CoderDojoBot!", "den Code der Coder an" };
        static List<ActivityType> _activityList = new List<ActivityType>() { ActivityType.Playing, ActivityType.Watching };

        public CustomStatusModule(AuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [Command("update-status1")]
        public async Task Status(ActivityType activity, [Remainder] string input)
        {
            if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await base.Context.Client.SetActivityAsync(new Game(input, type: activity));
                await ReplyAsync($"Der Status1 wurde auf **{activity} {input}** gesetzt!");
                _statusList[1] = input;
                _activityList[1] = activity;
            }
            else if (!_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await ReplyAsync($"Du bist nicht getrustet, {base.Context.User.Mention}");
            }
            else
            {
                await ReplyAsync("Ein Fehler ist aufgetreten! Bitte informiere die Admins darüber!");
            }
        }
        [Command("update-status2")]
        public async Task Status2(ActivityType activity, [Remainder] string input)
        {
            if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await ReplyAsync($"Der Status2 wurde auf **{activity} {input}** gesetzt!");
                _statusList[0] = input;
                _activityList[0] = activity;
            }
            else if (!_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await ReplyAsync($"Du bist nicht getrustet, {base.Context.User.Mention}");
            }
            else
            {
                await ReplyAsync("Ein Fehler ist aufgetreten! Bitte informiere die Admins darüber!");
            }
        }
        [Command("update-status")]
        public async Task StatusType(string input)
        {
            if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue) && input == "DnD" || input == "Do not Disturb" || input == "Bitte nicht stören")
            {
                await base.Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                await ReplyAsync("Der Status wurde auf **Bitte nicht stören** gesetzt!");
            }
            else if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue) && input == "Online" || input == "online")
            {
                await base.Context.Client.SetStatusAsync(UserStatus.Online);
                await ReplyAsync("Der Status wurde auf **Online** gesetzt!");
            }
            else if (_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue) && input == "AFK" || input == "Afk" || input == "afk")
            {
                await base.Context.Client.SetStatusAsync(UserStatus.AFK);
                await ReplyAsync("Der Status wurde auf **Abwesend** gesetzt!");
            }
            else if (!_authorizationService.IsUserTrusted(base.Context.User.Username + "#" + base.Context.User.DiscriminatorValue))
            {
                await ReplyAsync($"Du bist nicht getrustet, {base.Context.User.Mention}");
            }
            else
            {
                await ReplyAsync("Ein Fehler ist aufgetreten! Bitte informiere die Admins darüber!");
            }
        }
        [Command("startstatusupdater")]
        public async Task StatusUpdater()
        {
            await ReplyAsync("Der Statusupdater wurde gestartet!");
            while (true)
            {
                if (_abort == 1)
                {
                    await base.Context.Client.SetActivityAsync(null);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else if (_abort == 0)
                {
                    while (_abort == 0)
                    {
                        _timer = new Timer(async _ =>
                        {
                            await base.Context.Client.SetActivityAsync(new Game(_statusList.ElementAtOrDefault(_statusIndex), type: _activityList.ElementAtOrDefault(_statusIndex)));
                            _statusIndex = _statusIndex + 1 == _statusList.Count ? 0 : _statusIndex + 1;
                        },
                        null,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(10));
                    }
                }
                else
                {
                    await ReplyAsync("Ein Fehler ist aufgetreten! Bitte informiere die Admins darüber!");
                }
            }
        }

        [Command("stopstatus")]
        public async Task Stopstatus()
        {
            await ReplyAsync("Der Status wurde gestoppt!");
            _abort = 1;
        }

        [Command("startstatus")]
        public async Task Startstatus()
        {
            await ReplyAsync("Der Status wurde gestartet!");
            _abort = 0;
        }
    }
}
