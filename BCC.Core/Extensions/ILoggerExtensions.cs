using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BCC.Core.Extensions
{
    public static class ILoggerExtensions
    {
        public static void TryLog<T>(this ILogger<T> logger, LogLevel level, Exception ex, string message)
        {
            if (logger != null)
            {
                switch (level)
                {
                    case LogLevel.Trace:
                        logger.LogTrace(message);
                        break;
                    case LogLevel.Debug:
                        logger.LogDebug(message);
                        break;

                    case LogLevel.Information:
                        logger.LogInformation(message);
                        break;
                    case LogLevel.Warning:
                        logger.LogWarning(message);
                        break;
                    case LogLevel.Error:
                        logger.LogError(message);
                        break;
                    case LogLevel.Critical:
                        logger.LogCritical(message);
                        break;
                }

            }
        }
        public static void TryLog<T>(this ILogger<T> logger, LogLevel level, string message)
        {
            if (logger != null)
            {
                switch (level)
                {
                    case LogLevel.Trace:

                        break;
                    case LogLevel.Debug:
                        break;

                    case LogLevel.Information:

                        break;
                    case LogLevel.Warning:

                        break;
                    case LogLevel.Error:

                        break;
                    case LogLevel.Critical:

                        break;
                }

            }
        }
    }
}
