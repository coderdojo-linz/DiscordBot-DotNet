using Discord.Commands;

using DiscordBot.Modules.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services.Base
{
    public class CommandSuggestionsService
    {
        private readonly CommandService _commandService;

        public CommandSuggestionsService(CommandService commandService)
        {
            _commandService = commandService;
        }

        public async Task SuggestCommand(ICommandContext context, char prefix)
        {
            var cmd = context.Message.Content;
            cmd = cmd.TrimStart(prefix);
            cmd = cmd.Split(' ')[0];

            var availableCommands = _commandService.Commands.Select(x => x.Name).ToArray();

            await context.Channel.SendMessageAsync($"Dein Befehl wurde nicht erkannt!{CreateDidYouMean(availableCommands, cmd)}");
        }

        private string CreateDidYouMean(string[] commands, string wrongCommand)
        {
            string[] nearests = GetNearests(commands, wrongCommand, 5);
            int count = nearests.Length;

            if (count == 0) { return ""; }

            if (count <= 2)
            {
                return $" Meintest du: {string.Join(" oder ", nearests.Select(x => $"`{x}`"))}?";
            }

            return $" Meintest du einen der folgenden Befehle?\n{string.Join('\n', nearests.Select(x => $"- `{x}`"))}";
        }

        private string[] GetNearests(string[] list, string what, int radius = 0)
        {
            List<string> result = new List<string>();
            int lastDistance = int.MaxValue - 1;
            int currentDistance;
            foreach (string entry in list)
            {
                currentDistance = StringComparisonEx.GetLevenshteinDistance(entry, what);
                if (radius == 0 || currentDistance <= radius)
                {
                    if (lastDistance > currentDistance)
                    {
                        lastDistance = currentDistance;
                        result.Clear();
                        result.Add(entry);
                    }
                    else if (lastDistance == currentDistance)
                    {
                        lastDistance = currentDistance;
                        result.Add(entry);
                    }
                }
            }
            result.Sort();
            return result.ToArray();
        }
    }
}