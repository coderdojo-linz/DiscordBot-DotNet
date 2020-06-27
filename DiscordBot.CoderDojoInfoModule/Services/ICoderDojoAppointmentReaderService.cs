using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.Domain.CoderDojoInfoModule.DataModel;

namespace DiscordBot.Domain.CoderDojoInfoModule.ServicesImpl {
    public interface ICoderDojoAppointmentReaderService {
        Task<List<CoderDojoAppointment>> ReadCurrentAppointments();
    }
}