using Discord;
using Discord.Commands;

using DiscordBot.Modules.Utils.Extensions;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _map;

        public HelpModule(IServiceProvider map, CommandService commands)
        {
            _commands = commands;
            _map = map;
        }

        [Command("help")]
        [Summary("Lists this bot's commands.")]
        public async Task Help(string path = "")
        {
            EmbedBuilder output = new EmbedBuilder();
            if (path == "")
            {
                output.Title = "Dojo Bot - help";
                foreach (var mod in _commands.Modules.Where(m => m.Parent == null))
                {
                    AddHelp(mod, output);
                }

                output.Footer = new EmbedFooterBuilder
                {
                    Text = "Use 'help <module>' to get help with a module."
                };
            }
            else
            {
                var mod = _commands.Modules.FirstOrDefault(m => string.Equals(m.Group, path, StringComparison.OrdinalIgnoreCase));
                if (mod != null)
                {
                    var descAttr = (DescriptionAttribute)mod.Attributes.First(a => a is DescriptionAttribute);
                    string desc = "";
                    if (descAttr != null)
                        desc = descAttr.Text + "\n";
                    output.Title = mod.FriendlyName();
                    output.Description = $"{mod.Summary}\n{desc}\n" +
                                         (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
                                         (mod.Aliases.Any() ? $"Prefix(es): {string.Join(",", mod.Aliases)}\n" : "") +
                                         (mod.Submodules.Any() ? $"Submodules: {mod.Submodules.Select(m => m)}\n" : "") + " ";
                    AddCommands(mod, output);
                }

                var command = mod != null ? null : _commands.Modules
                    .Where(x => string.IsNullOrEmpty(x.Group) && !x.IsSubmodule)
                    .SelectMany(x => x.Commands)
                    .FirstOrDefault(x => string.Equals(x.Name, path, StringComparison.InvariantCultureIgnoreCase));

                if (command != null)
                {
                    AddCommand(command, output);
                    //var sb = new StringBuilder();
                    //if (command.Aliases.Any())
                    //{
                    //    sb.AppendLine($"Aliases: {string.Join(", ", command.Aliases)}");
                    //}

                    //if (!string.IsNullOrEmpty(command.Summary))
                    //{
                    //    sb.AppendLine($"Summary: {command.Summary}");
                    //}

                    //output.AddField(f =>
                    //{
                    //    f.Name = $"**{command.Name}**";
                    //    f.Value = sb.ToString();
                    //});
                }

                if (mod == null && command == null)
                {
                    await ReplyAsync("No module or command could be found with that name.");
                    return;
                }
            }

            await ReplyAsync("", embed: output.Build());
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
                sb.AppendLine($"Submodules: {string.Join(", ", subModules.Select(x => x.Name))}");
            }

            if (commands.Any())
            {
                sb.AppendLine($"Commands: {string.Join(", ", commands)}");
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
            var prefix = GetPrefix(command);
            var aliases = GetAliases(command);
            var usage = string.Join(" ", prefix, aliases).Replace(" ", "");

            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n" +
                (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "") +
                (command.Aliases.Any() ? $"**Aliases:** {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                $"**Usage:** `{usage}`";
            });
        }

        public string GetAliases(CommandInfo command)
        {
            StringBuilder output = new StringBuilder();
            if (!command.Parameters.Any()) return output.ToString();
            foreach (var param in command.Parameters)
            {
                if (param.IsOptional)
                    output.Append($"[{param.Name} = {param.DefaultValue}] ");
                else if (param.IsMultiple)
                    output.Append($"|{param.Name}| ");
                else if (param.IsRemainder)
                    output.Append($"...{param.Name} ");
                else
                    output.Append($"<{param.Name}> ");
            }
            return output.ToString();
        }

        public string GetPrefix(CommandInfo command)
        {
            var output = GetPrefix(command.Module);
            output += $"{command.Aliases.FirstOrDefault()} ";
            return output;
        }

        public string GetPrefix(ModuleInfo module)
        {
            string output = "";
            if (module.Parent != null) output = $"{GetPrefix(module.Parent)}{output}";
            if (module.Aliases.Any())
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            return output;
        }
    }
}