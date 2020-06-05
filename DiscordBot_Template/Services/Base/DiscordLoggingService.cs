using Discord;
using Discord.Commands;

using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace CoderDojo_Discordbot.Services
{
    public class DiscordLoggingService : IDisposable
    {
        private readonly CommandService _commandService;
        private readonly ILogger<IDiscordClient> _logger;

        public DiscordLoggingService
        (
            CommandService commandService,
            ILogger<IDiscordClient> logger
        )
        {
            _commandService = commandService;
            _logger = logger;

            _commandService.Log += OnLog;
        }

        private System.Threading.Tasks.Task OnLog(LogMessage arg)
        {
            var logLevel = arg.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };

            _logger.Log(logLevel, 0, arg.Exception, arg.Message, null);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _commandService.Log -= OnLog;
        }
    }
}