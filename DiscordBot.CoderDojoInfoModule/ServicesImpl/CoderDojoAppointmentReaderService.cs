using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordBot.Domain.CoderDojoInfoModule.Configuration;
using DiscordBot.Domain.CoderDojoInfoModule.DataModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiscordBot.Domain.CoderDojoInfoModule.ServicesImpl {
    public class CoderDojoAppointmentReaderService : ICoderDojoAppointmentReaderService {
        private CDAppointmentSettings Settings { get; }
        public HttpClient WebClient { get; }
        
        public CoderDojoAppointmentReaderService(IOptions<CDAppointmentSettings> settings) {
            this.Settings = settings?.Value;
            this.WebClient = new HttpClient();
        }
        
        public async Task<List<CoderDojoAppointment>> ReadCurrentAppointments() {
            var AppointmentUrl = Settings?.NextAppointmentsUrl ?? "https://cdw-planner.azurewebsites.net/api/events?past=false";
            var response = await WebClient.GetAsync(AppointmentUrl);

            var items = JsonConvert.DeserializeObject<List<CoderDojoAppointment> >(await response.Content.ReadAsStringAsync());

            // Just to make sure - sort the Entries in Ascending order by date.
            items.Sort((item1, item2) => item1.Date.CompareTo(item2.Date));

            return items;
        }
    }
}