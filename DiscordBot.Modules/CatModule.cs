using Discord.Commands;
using DiscordBot.Modules.Services;
using Newtonsoft.Json;

using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class CatModule : ModuleBase
    {
        private readonly ICatService _catService;

        public CatModule(ICatService catService) => _catService = catService;

        [Command("cat")]
        public async Task Cat() => await base.Context.Channel.SendFileAsync(await _catService.GetCatAsync(), "cat.jpg");
    }

}