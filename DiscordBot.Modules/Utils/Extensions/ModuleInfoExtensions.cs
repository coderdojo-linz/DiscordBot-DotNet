using Discord.Commands;

namespace DiscordBot.Modules.Utils.Extensions
{
    public static class ModuleInfoExtensions
    {
        public static string FriendlyName(this ModuleInfo info)
        {
            return info.Name.Replace("Module", "");
        }
    }
}
