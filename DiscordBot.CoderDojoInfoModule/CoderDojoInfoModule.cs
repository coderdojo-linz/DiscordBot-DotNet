using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Domain.CoderDojoInfoModule.Controller;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Domain.CoderDojoInfoModule {
    public class CoderDojoInfoModule : ModuleBase {
        private static IConfiguration config;
        
        static CoderDojoInfoModule() {
            // read settings if there are any.
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
                        
        }
        
        [Command("termine")]
        public async Task NextAppointments(IGuildUser user) {
            string url = config["nextAppointmentsUrl"] ?? "https://participants-management-service.azurewebsites.net/api/events/?past=false";

            var parser = new CoderDojoAppointmentReader(new Uri(url));
            var appointments = await parser.ReadCurrentAppointments();
            
            StringBuilder response = new StringBuilder();
            response.Append($"Hier die Termine für dich, {user.Nickname}:  \n"); // the two blanks in front of linebreak are needed because discord uses Markdown
            foreach (var appointment in appointments) {
                response.Append($"> {appointment.Date.ToString("dddd, dd.MM.yyyy", new CultureInfo("de-DE"))}  \n");
            }

            await ReplyAsync(response.ToString());
        }
    }
}