using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordBot.Domain.CoderDojoInfoModule.DataModel;
using Newtonsoft.Json;

namespace DiscordBot.Domain.CoderDojoInfoModule.Controller {
    public class CoderDojoAppointmentReader {
        private Uri PageUrl { get; }
        public HttpClient WebClient { get; }

        public CoderDojoAppointmentReader(Uri pageUrl) {
            this.PageUrl = pageUrl;
            this.WebClient = new HttpClient();
        }


        public async Task<List<CoderDojoAppointment>> ReadCurrentAppointments() {
            var response = await WebClient.GetAsync(PageUrl);

            var items = JsonConvert.DeserializeObject<List<CoderDojoAppointment> >(await response.Content.ReadAsStringAsync());

            // Just to make sure - sort the Entries in Ascending order by date.
            items.Sort((item1, item2) => item1.Date.CompareTo(item2.Date));

            return items;
        }
    }
}