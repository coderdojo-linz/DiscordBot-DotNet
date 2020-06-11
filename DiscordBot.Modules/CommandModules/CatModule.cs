using Discord.Commands;

using DiscordBot.Modules.Services;

using System.Threading.Tasks;

namespace DiscordBot.Modules.CommandModules
{
    public class CatModule : ModuleBase
    {
        private readonly ICatService _catService;

        public CatModule(ICatService catService) => _catService = catService;

        [Command("cat"), Summary("Shows us cats")]
        public async Task Cat() => await base.Context.Channel.SendFileAsync(await _catService.GetCatAsync(), "cat.jpg");

        [Command("more-cat"), Summary("Shows us more cats")]
        public async Task MoreCat() => await base.Context.Channel.SendFileAsync(await _catService.GetCatAsync(), "cat.jpg");
    }
}