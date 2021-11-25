using Discord;
using Discord.Commands;

using DiscordBot.Domain.Configuration;
using DiscordBot.Modules.Utils;
using DiscordBot.Modules.Utils.Extensions;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class HelpModule : ModuleBase
    {
        private readonly IServiceProvider _map;
        private readonly CommandService _commands;
        private readonly IOptions<DiscordSettings> _configuration;

        public HelpModule(IServiceProvider map, CommandService commands, IOptions<DiscordSettings> configuration)
        {
            _map = map;
            _commands = commands;
            _configuration = configuration;
        }

        [Command("help")]
        //[Alias("h")]
        [Summary("Zeigt die Hilfe an.")]
        public async Task Help(string path = "")
        {
            var output = path == ""
                ? GetDefaultOutput()
                : await GetOutputForCommand(path);

            if (output == null) return;

            await ReplyAsync(embed: output.Build());
        }

        private async Task<EmbedBuilder> GetOutputForCommand(string path)
        {
            var output = new EmbedBuilder().WithColor(0x129AD4);

            var (modules, commands) = FindModuleOrCommand(path);

            if (!modules.Any() && !commands.Any())
            {
                (modules, commands) = FindModuleOrCommand(path, true);
            }

            if (modules.Any())
            {
                var module = modules.First();

                var descAttr = (DescriptionAttribute)module.Attributes.FirstOrDefault(a => a is DescriptionAttribute);

                var description = "";
                if (descAttr != null)
                    description = descAttr.Text + "\n";

                output.Title = module.FriendlyName();
                output.Description =
                    $"{module.Summary}\n{description}\n" +
                        (!string.IsNullOrEmpty(module.Remarks) ? $"({module.Remarks})\n" : "") +
                        (module.Aliases.Any() ? $"Prefix(e): {string.Join(", ", module.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                        (module.Submodules.Any() ? $"Submodule: {module.Submodules.Select(m => m)}\n" : "") + " ";

                AddCommands(module, output);
            }

            if (commands.Any())
            {
                AddCommand(commands.First(), output);
            }

            if (!modules.Any() && !commands.Any())
            {
                await ReplyAsync("Kein Modul oder Befehl wurde mit diesem Namen gefunden.");
                return null;
            }

            return output;
        }

        private (IEnumerable<ModuleInfo> modules, IEnumerable<CommandInfo> commands) FindModuleOrCommand(string searchTerm, bool useLevenstein = false)
        {
            var modules = _commands.Modules;

            var commands = _commands.Modules
                .Where(x => string.IsNullOrEmpty(x.Group) && !x.IsSubmodule)
                .SelectMany(x => x.Commands);

            if (useLevenstein)
            {
                var moduleResults = modules
                    .Select(m => (item: m, distance: ComputeDistance(searchTerm, m.Group, m.Aliases)))
                    .OrderBy(x => x.Item2)
                    .Select(x => x.item);

                var commandResults = commands
                    .Select(m => (item: m, distance: ComputeDistance(searchTerm, m.Name, m.Aliases)))
                    .OrderBy(x => x.Item2)
                    .Select(x => x.item);

                return (moduleResults, commandResults);
            }

            var module = modules
                .Where(x => Compare(searchTerm, x.Aliases.Append(x.Group)));

            var command = commands
                .Where(x => Compare(searchTerm, x.Aliases.Append(x.Name)));

            return (module, command);

            static bool Compare(string searchTerm, IEnumerable<string> values)
            {
                return values.Any(x => string.Equals(searchTerm, x, StringComparison.OrdinalIgnoreCase));
            }

            static int ComputeDistance(string searchTerm, string name, IReadOnlyList<string> aliases)
            {
                var smallestDistance = aliases.AsEnumerable().Append(name)
                    .Select(x => StringComparisonEx.GetLevenshteinDistance(x, searchTerm))
                    .Min();

                return smallestDistance;
            }
        }

        private EmbedBuilder GetDefaultOutput()
        {
            var output = new EmbedBuilder().WithColor(0x129AD4);
            output.Title = "Dojo Bot - Hilfe";
            foreach (var mod in _commands.Modules.Where(m => m.Parent == null))
            {
                AddHelp(mod, output);
            }

            output.Footer = new EmbedFooterBuilder
            {
                Text = "Verwende 'help <module>' um Hilfe für ein Modul zu bekommen."
            };
            return output;
        }

        public void AddHelp(ModuleInfo module, EmbedBuilder builder)
        {
            foreach (var sub in module.Submodules)
            {
                AddHelp(sub, builder);
            }

            var sb = new StringBuilder();
            var commands = module.Commands.Select(x => $"`{x.Name}`").ToList();
            var subModules = module.Submodules.Select(m => m).ToList();

            if (!string.IsNullOrEmpty(module.Group))
            {
                sb.AppendLine($"Prefix: `{module.Group}`");
            }

            if (subModules.Any())
            {
                sb.AppendLine($"Submodule: {string.Join(", ", subModules.Select(x => x.Name))}");
            }

            if (commands.Any())
            {
                sb.AppendLine($"Befehle: {string.Join(", ", commands)}");
            }

            if (!string.IsNullOrEmpty(module.Summary))
            {
                sb.AppendLine($"Summary: {module.Summary}");
            }

            builder.AddField(f =>
            {
                f.Name = $"**{module.FriendlyName()}**";
                f.Value = sb.ToString();
            });
        }

        public void AddCommands(ModuleInfo module, EmbedBuilder builder)
        {
            foreach (var command in module.Commands)
            {
                command.CheckPreconditionsAsync(Context, _map).GetAwaiter().GetResult();
                AddCommand(command, builder);
            }
        }

        public void AddCommand(CommandInfo command, EmbedBuilder builder)
        {
            List<string> commandNameList = new List<string>();
            AddPrefix(command, commandNameList);
            List<string> parameters = GetParameters(command);

            string usage = string.Join(" ", commandNameList.Concat(parameters));

            var descAttr = (DescriptionAttribute)command.Attributes.FirstOrDefault(a => a is DescriptionAttribute);
            string desc = "";
            if (descAttr != null)
                desc = descAttr.Text + "\n";

            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n{desc}\n" +
                (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "") +
                (command.Aliases.Any() ? $"**Aliases:** {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                $"**Verwendung:** `{_configuration.Value.CommandPrefix}{usage}`";
            });
        }

        public List<string> GetParameters(CommandInfo command)
        {
            List<string> output = new List<string>();

            foreach (var param in command.Parameters)
            {
                if (param.IsOptional)
                {
                    var defaultValue = param.DefaultValue;
                    output.Add($"[{param.Name}" + ((string)defaultValue != "" && defaultValue != null ? $" = '{param.DefaultValue}'" : "") + "]");
                }
                else if (param.IsMultiple)
                    output.Add($"<{param.Name}->");
                else if (param.IsRemainder)
                    output.Add($"<...{param.Name}>");
                else
                    output.Add($"<{param.Name}>");
            }
            return output;
        }

        public void AddPrefix(CommandInfo command, List<string> output)
        {
            AddPrefix(command.Module, output);
            output.Add(command.Aliases.First());
        }

        public void AddPrefix(ModuleInfo module, List<string> output)
        {
            if (module.Parent != null)
            {
                AddPrefix(module.Parent, output);
                output.Add(module.Aliases.First());
            }
        }
    }
}