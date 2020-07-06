﻿using Discord.Commands;

using DiscordBot.Domain;
using DiscordBot.Domain.Database;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Modules.CommandModules
{
    [Group("code")]
    [Summary("Speichert und zeigt Code-Snippets an.")]
    public class CodeSnippetModule : ModuleBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileExplorerModule> _logger;
        private readonly DatabaseContainer<SnippetInfo> _container;

        public CodeSnippetModule
        (
            IConfiguration configuration,
            ILogger<FileExplorerModule> logger,
            IDatabaseService databaseService
        )
        {
            _configuration = configuration;
            _logger = logger;
            _container = databaseService.GetContainer<SnippetInfo>("codesnippets");
        }

        [Command("add")]
        [Alias("+")]
        public async Task AddCodeAsync(string name, [Remainder] string code)
        {
            var info = new SnippetInfo()
            {
                UserId = base.Context.User.Id,
                Name = name,
                Code = code,
                Id = Guid.NewGuid().ToString()
            };

            try
            {
                AddEntryToDatabase(info);

                await ReplyAsync($"Der Code wurde unter dem Namen `{name}` gespeichert.");
            }
            catch
            {
                await ReplyAsync("Ein Code mit diesem Namen existiert bereits!");
            }

        }

        [Command("edit")]
        public async Task EditCodeAsync(string name, [Remainder] string newCode)
        {
            SnippetInfo info;
            try
            {
                info = GetCodeInfoForName(name);
            }
            catch
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }

            if (info.UserId != base.Context.User.Id)
            {
                await ReplyAsync($"Du kannst keinen Code von anderen bearbeiten!");
                return;
            }

            UpdateDb(name, newCode);
            await ReplyAsync("Der Code wurde erfolgreich geändert.");
        }

        [Command("remove")]
        [Alias("delete", "del", "rm", "-")]
        public async Task RemoveCodeAsync(string name)
        {
            SnippetInfo info;
            try
            {
                info = GetCodeInfoForName(name);
            }
            catch
            {
                await ReplyAsync($"Es wurde kein Code unter dem Namen `{name}` gefunden!");
                return;
            }

            if (info.UserId != base.Context.User.Id)
            {
                await ReplyAsync($"Du kannst keinen Code von anderen löschen!");
                return;
            }

            RemoveFromDb(name);
            await ReplyAsync("Der Code wurde erfolgreich gelöscht.");
        }

        [Command("list")]
        [Alias("ls")]
        public async Task ListCodeAsync(string showInfoRaw = "")
        {
            var codes = ReadDatabase();
            int count = codes.Length;

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
                    string entry = $"**{entryCounter++})** `{x.Name}`";

                    if (!showInfo)
                    {
                        return entry;
                    }
                    var rawUser = await base.Context.Guild.GetUserAsync(x.UserId);
                    string userTag = $"`{rawUser.Username}#{rawUser.Discriminator}`";
                    string info = $" - Erstellt von: {userTag} - Länge: {x.Code.Length} Zeichen";
                    return entry + info;
                })
                .Select(x => x.Result) // get the result of async operation. IMPORTANT!!!!
                .ToArray();

            await ReplyAsync($"{string.Join('\n', entries)}\n{entryCount}");
        }

        [Command("info")]
        public async Task ShowCodeInfoAsync([Remainder] string name)
        {
            var info = GetCodeInfoForName(name);
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
            var code = GetCodeInfoForName(name);
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
        private void AddEntryToDatabase(SnippetInfo info)
        {
            _container.Insert(info);
        }

        private SnippetInfo[] ReadDatabase()
        {
            return _container.Query("SELECT * FROM db");
        }

        private SnippetInfo GetCodeInfoForName(string name)
        {
            return _container.Query($"SELECT * FROM db WHERE db.Name = '{name}'").FirstOrDefault();
        }

        private void UpdateDb(string name, string code)
        {
            var info = GetCodeInfoForName(name);
            info.Code = code;
            _container.Upsert(info);
        }

        private void RemoveFromDb(string name)
        {
            _container.Delete(GetCodeInfoForName(name));
        }
    }
}
