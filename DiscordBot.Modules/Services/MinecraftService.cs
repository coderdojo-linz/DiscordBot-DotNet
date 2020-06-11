using LibMCRcon.RCon;
using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Domain.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace DiscordBot.Modules.Services
{
    public class MinecraftService : IMinecraftService
    {
        private TCPRcon rcon;
        private MinecraftSettings settings;

        public MinecraftService(IOptions<MinecraftSettings> s)
        {
            settings = s.Value;
            EnsureConnectedAsync();
        }

        public void Dispose()
        {
            rcon.StopComms();
        }

        public async Task<string> ExecuteCommandAsync(string command)
        {
            await EnsureConnectedAsync();
            if (!rcon.IsConnected)
            {
                return "No Connection to the given Server available.";
            }

            return await rcon.ExecuteCmdAsync(command);
        }

        private async ValueTask EnsureConnectedAsync()
        {
            if (rcon != null && rcon.IsConnected)
            {
                return;
            }

            for (int i = 0; i < 10; i++)
            {
                if (rcon != null && rcon.IsConnected)
                {
                    return;
                }

                rcon = new TCPRcon(settings.IP, settings.Port, settings.Password);
                rcon.StartComms();
                await Task.Delay(5000);
            }

            return;
        }
    }
}
