using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Infrastructure.Loggers
{
    public class LoggerHelper<T>: ILoggerHelper<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerHelper(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, message);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }

}
