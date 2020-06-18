using System;
using Newtonsoft.Json;

// Data Object for JSON Parsing
namespace DiscordBot.Domain.CoderDojoInfoModule.DataModel {
        public class CoderDojoAppointment
        {
            [JsonProperty("_id")]
            public string Id { get; set; }

            [JsonProperty("date")]
            public DateTime Date { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }
        }
}