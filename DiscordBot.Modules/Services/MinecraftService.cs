using DiscordBot.Domain.Configuration;

using LibMCRcon.RCon;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public class MinecraftService : IDisposable
    {
        private TCPRcon rcon;
        private MinecraftSettings settings;

        public MinecraftService(IOptions<MinecraftSettings> s)
        {
            settings = s.Value;
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

        public async Task<bool> EnqueueCommandsAsync(IEnumerable<string> commands)
        {
            await EnsureConnectedAsync();
            if (!rcon.IsConnected)
            {
                return false;
            }

            foreach (var command in commands)
            {
                rcon.QueCommand(command);
            }

            return true;
        }

        private async ValueTask EnsureConnectedAsync()
        {
            if (rcon == null)
            {
                StartRcon();
                await Task.Delay(100);
            }

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

                StartRcon();
                await Task.Delay(5000);
            }

            return;
        }

        private void StartRcon()
        {
            rcon = new TCPRcon(settings.IP, settings.Port, settings.Password);
            rcon.StartComms();
        }

        public void Dispose()
        {
            rcon?.StopComms();
            rcon = null;
        }
    }
}