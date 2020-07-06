using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Domain.CoderDojoInfoModule.ServicesImpl;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Domain.CoderDojoInfoModule {
    public class CoderDojoInfoModule : ModuleBase {
        public ICoderDojoAppointmentReaderService ReaderService { get; }
        public ILogger<CoderDojoInfoModule> Logger { get; }

        public CoderDojoInfoModule(ICoderDojoAppointmentReaderService readerService, ILogger<CoderDojoInfoModule> logger) {
            ReaderService = readerService;
            Logger = logger;
        }
        
        [Command("termine")]
        public async Task NextAppointments() {

            try {
                var appointments = await ReaderService.ReadCurrentAppointments();

                StringBuilder response = new StringBuilder();

                // the two blanks in front of linebreak are needed because discord uses Markdown
                response.Append($"Hier die Termine für dich, {Context.User.Mention}: \n"); 
                foreach (var appointment in appointments) {
                    response.Append($"> {appointment.Date.ToString("dddd, dd.MM.yyyy", new CultureInfo("de-DE"))}  \n");
                }

                await ReplyAsync(response.ToString());
            }
            catch (Exception ex) {
                Logger.LogError($"Die Termine konnte nicht eingeholt werden: {ex}");
                await ReplyAsync("Leider kann ich dir die Termine im Moment " +
                                 "nicht sagen, weil ein Fehler aufgetreten ist .....");
            }
        }
    }
}