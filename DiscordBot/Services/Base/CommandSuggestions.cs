using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services.Base
{
    internal class CommandSuggestionsService
    {
        public static async Task SuggestCommand(CommandService commandService, ICommandContext context, char prefix)
        {
            var cmd = context.Message.Content;
            cmd = cmd.TrimStart(prefix);
            cmd = cmd.Split(' ')[0];

            var availableCommands = commandService.Commands.Select(x => x.Name).ToArray();

            await context.Channel.SendMessageAsync($"Dein Befehl wurde nicht erkannt!{CreateDidYouMean(availableCommands, cmd)}");
        }

        private static string CreateDidYouMean(string[] commands, string wrongCommand)
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

        private static string[] GetNearests(string[] list, string what, int radius = 0)
        {
            List<string> result = new List<string>();
            int lastDistance = int.MaxValue - 1;
            int currentDistance;
            foreach (string entry in list)
            {
                currentDistance = GetLevenshteinDistance(entry, what);
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

        private static int GetLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++) { }

            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}