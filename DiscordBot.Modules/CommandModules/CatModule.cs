using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Modules.Services;

namespace DiscordBot.Modules.CommandModules
{
    public class CatModule : ModuleBase
    {
        private readonly ICatService _catService;

        public CatModule(ICatService catService) => _catService = catService;

        [Command("cat")]
        public async Task Cat() => await base.Context.Channel.SendFileAsync(await _catService.GetCatAsync(), "cat.jpg");
    }

}