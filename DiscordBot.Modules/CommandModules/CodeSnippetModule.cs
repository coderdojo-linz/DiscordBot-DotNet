using Discord.Commands;

using DiscordBot.Domain;
using DiscordBot.Domain.Configuration;

using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DiscordBot.Modules.CommandModules
{
    [Group("code")]
    [Summary("Saves and shows code snippets.")]
    public class CodeSnippetModule : ModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileExplorerModule> _logger;

        public CodeSnippetModule
        (
            IOptions<DiscordSettings> options,
            IConfiguration configuration,
            ILogger<FileExplorerModule> logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Command("add")]
        [Alias("+")]
        public async Task AddCodeAsync(string name, [Remainder] string code)
        {
            var info = new SnippetInfo()
            {
                UserId = base.Context.User.Id,
                Name = name,
                Code = code
            };
            if (await CodeWithNameExists(name))
            {
                await ReplyAsync("Ein Code mit diesem Namen existiert bereits!");
                return;
            }

            await AddEntryToDatabase(info);
            await ReplyAsync($"Der Code wurde unter dem Namen `{name}` gespeichert.");
        }

        [Command("edit")]
        public async Task EditCodeAsync(string name, [Remainder] string newCode)
        {
            var info = await GetCodeInfoForName(name);
            if (info == null)
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }
            if (info.UserId != base.Context.User.Id)
            {
                await ReplyAsync($"Du kannst keinen Code von anderen bearbeiten!");
                return;
            }
            info.Code = newCode;
            await UpdateDb(name, info);
            await ReplyAsync("Der Code wurde erfolgreich geändert.");
        }

        [Command("remove")]
        [Alias("delete", "del", "rm", "-")]
        public async Task RemoveCodeAsync(string name)
        {
            var info = await GetCodeInfoForName(name);
            if (info == null)
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }
            if (info.UserId != base.Context.User.Id)
            {
                await ReplyAsync($"Du kannst keinen Code von anderen löschen!");
                return;
            }
            await RemoveFromDb(name);
            await ReplyAsync("Der Code wurde erfolgreich gelöscht.");
        }

        [Command("list")]
        [Alias("ls")]
        public async Task ListCodeAsync(string showInfoRaw = "")
        {
            var codes = await ReadDatabase();
            int count = codes.Count;

            if (count == 0)
            {
                await ReplyAsync("Es existieren kein Code-Snippets.");
                return;
            }

            var showInfo = showInfoRaw == "info" || showInfoRaw == "-l";

            string entryCount = $"Insgesammt **{count}** " + (count == 1 ? "Eintrag." : "Einträge.");
            int entryCounter = 1;
            string[] entries = codes
                .Select(async x => {
                    string entry = $"**{entryCounter++})** `{x.Value.Name}`";

                    if (!showInfo)
                    {
                        return entry;
                    }
                    var rawUser = await base.Context.Guild.GetUserAsync(x.Value.UserId);
                    string userTag = $"`{rawUser.Username}#{rawUser.Discriminator}`";
                    string info = $" - Erstellt von: {userTag} - Länge: {x.Value.Code.Length} Zeichen";
                    return entry + info;
                })
                .Select(x => x.Result) // get the result of async operation. IMPORTANT!!!!
                .ToArray();

            await ReplyAsync($"{string.Join('\n', entries)}\n{entryCount}");
        }

        [Command("info")]
        public async Task ShowCodeInfoAsync([Remainder] string name)
        {
            var info = await GetCodeInfoForName(name);
            if (info == null)
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }

            var rawUser = await base.Context.Guild.GetUserAsync(info.UserId);
            string userTag = $"{rawUser.Username}#{rawUser.Discriminator}";
            string createdBy = $"_Erstellt von:_ `{userTag}`";

            string length = $"_Länge:_ {info.Code.Length} Zeichen";

            string output = $"**Informationen über `{name}`:**\n{createdBy}\n{length}";
            await ReplyAsync(output);
        }

        [Command("show")]
        public async Task ShowCodeAsync([Remainder] string name)
        {
            var code = await GetCodeInfoForName(name);
            if (code == null)
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }
            await ReplyAsync($"```cs\n{code.Code}\n```");
        }

        /// <summary>
        /// 1. LoadDatabase
        /// 2. Append Data
        /// 3. Save Database
        /// </summary>
        /// <returns></returns>
        public async Task AddEntryToDatabase(SnippetInfo info)
        {
            var db = await ReadDatabase();
            db.Add(info.Name, info);

            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "CodeModule"));
            var serialized = JsonConvert.SerializeObject(db);
            await File.WriteAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "CodeDb.json"), serialized);
        }

        public async Task<Dictionary<string, SnippetInfo>> ReadDatabase()
        {
            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return null;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "CodeModule"));
            if (!File.Exists(Path.Combine(pollModuleDirectory.FullName, "CodeDb.json")))
            {
                return new Dictionary<string, SnippetInfo>();
            }

            var content = await File.ReadAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "CodeDb.json"));
            return JsonConvert.DeserializeObject<Dictionary<string, SnippetInfo>>(content);
        }

        private async Task UpdateDb(string name, SnippetInfo info)
        {
            var db = await ReadDatabase();
            db[name] = info;

            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "CodeModule"));
            var serialized = JsonConvert.SerializeObject(db);
            await File.WriteAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "CodeDb.json"), serialized);
        }

        private async Task RemoveFromDb(string name)
        {
            var db = await ReadDatabase();
            db.Remove(name);

            var filesFolder = _configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                _logger.LogError("FileStore:DataFolder does not refer to an existing directory. Did you configure it in appsettings.json?");
                await ReplyAsync("Bot not properly configured. Check logs for details.");
                return;
            }

            var pollModuleDirectory = Directory.CreateDirectory(Path.Combine(filesFolder, "CodeModule"));
            var serialized = JsonConvert.SerializeObject(db);
            await File.WriteAllTextAsync(Path.Combine(pollModuleDirectory.FullName, "CodeDb.json"), serialized);
        }

        public async Task<SnippetInfo> GetCodeInfoForName(string name)
        {
            var db = await ReadDatabase();
            return db.GetValueOrDefault(name);
        }

        public async Task<bool> CodeWithNameExists(string name)
        {
            var db = await ReadDatabase();
            return db.Any(x => x.Value.Name == name);
        }
    }
}
