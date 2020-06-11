using System;
using System.Collections.Generic;
using System.Text;
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
