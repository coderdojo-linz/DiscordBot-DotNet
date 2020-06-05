using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Modules.WeatherModel
{
    public partial class CloudsModel
    {
        [JsonProperty("all")]
        public long All { get; set; }
    }
}
