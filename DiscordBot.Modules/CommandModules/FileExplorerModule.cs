using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class FileExplorerModule : ModuleBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<FileExplorerModule> logger;

        public FileExplorerModule(IConfiguration configuration, ILogger<FileExplorerModule> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Command("files")]
        public async Task FilesAsync([Remainder] string commandText)
        {
            var filesFolder = configuration["FileStore:DataFolder"];
            if (!Directory.Exists(filesFolder))
            {
                logger.LogError("FileStore:DataFolder referenziert kein existierendes Verzeichnis. Hast du es in appsettings.json konfiguriert?");
                await ReplyAsync("Der bot wurde nicht richtig konfiguriert. Siehe Logs für mehr Infos.");
                return;
            }

            if (commandText == "ls")
            {
                var files = Directory.EnumerateFileSystemEntries(filesFolder);
                await ReplyAsync(files.Aggregate(string.Empty, (agg, f) => $"{agg}\n{f}"));
                return;
            }

            var regex = new Regex("^cat ([^/ \\\\]+)$");
            if (regex.IsMatch(commandText))
            {
                var fileName = commandText.Substring("cat ".Length);
                fileName = Path.Combine(filesFolder, fileName);
                var fileContent = await File.ReadAllTextAsync(fileName);
                await ReplyAsync(fileContent.Substring(0, Math.Min(fileContent.Length, 256)));
                return;
            }

            logger.LogWarning($"Ungültige Eingabe: {commandText}");
            await ReplyAsync("Sorry, ich verstehe dich nicht.");
        }
    }
}