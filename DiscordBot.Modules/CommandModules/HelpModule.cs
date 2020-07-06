using Discord;
using Discord.Commands;
using DiscordBot.Domain.Configuration;
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
        [Summary("Zeigt die Hilfe an.")]
        public async Task Help(string path = "")
        {
            EmbedBuilder output = new EmbedBuilder();
            if (path == "")
            {
                output.Title = "Dojo Bot - Hilfe";
                foreach (var mod in _commands.Modules.Where(m => m.Parent == null))
                {
                    AddHelp(mod, output);
                }

                output.Footer = new EmbedFooterBuilder
                {
                    Text = "Verwende 'help <module>' um Hilfe für ein Modul zu bekommen."
                };
            }
            else
            {
                var mod = _commands.Modules.FirstOrDefault(m => string.Equals(m.Group, path, StringComparison.OrdinalIgnoreCase));
                if (mod != null)
                {
                    var descAttr = (DescriptionAttribute)mod.Attributes.FirstOrDefault(a => a is DescriptionAttribute);
                    string desc = "";
                    if (descAttr != null)
                        desc = descAttr.Text + "\n";
                    output.Title = mod.FriendlyName();
                    output.Description = $"{mod.Summary}\n{desc}\n" +
                                         (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
                                         (mod.Aliases.Any() ? $"Prefix(e): {string.Join(", ", mod.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                                         (mod.Submodules.Any() ? $"Submodule: {mod.Submodules.Select(m => m)}\n" : "") + " ";
                    AddCommands(mod, output);
                }

                var command = mod != null ? null : _commands.Modules
                    .Where(x => string.IsNullOrEmpty(x.Group) && !x.IsSubmodule)
                    .SelectMany(x => x.Commands)
                    .FirstOrDefault(x => string.Equals(x.Name, path, StringComparison.InvariantCultureIgnoreCase));

                if (command != null)
                {
                    AddCommand(command, output);
                }

                if (mod == null && command == null)
                {
                    await ReplyAsync("Kein Modul oder Befehl wurde mit diesem Namen gefunden.");
                    return;
                }
            }

            await ReplyAsync(embed: output.Build());
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

            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n" +
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
                    output.Add($"[{param.Name}" + (param.DefaultValue.ToString() != "" ? $" = '{param.DefaultValue}'" : "") + "]");
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