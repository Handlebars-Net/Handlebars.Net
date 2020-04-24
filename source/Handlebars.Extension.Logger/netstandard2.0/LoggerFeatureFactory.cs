using HandlebarsDotNet.Features;
using Microsoft.Extensions.Logging;

namespace HandlebarsDotNet.Extension.Logger
{
    internal partial class LoggerFeatureFactory : IFeatureFactory
    {
        public LoggerFeatureFactory(ILoggerFactory factory)
        {
            _logger = CreateLogger(factory);
        }

        private static Log CreateLogger(ILoggerFactory factory)
        {
            static LogLevel LogLevelMapper(LoggingLevel level) =>
                level switch
                {
                    LoggingLevel.Debug => LogLevel.Debug,
                    LoggingLevel.Info => LogLevel.Information,
                    LoggingLevel.Warn => LogLevel.Warning,
                    LoggingLevel.Error => LogLevel.Error,
                    _ => LogLevel.Information
                };

            return (arguments, level, format) =>
            {
                var logLevel = LogLevelMapper(level);
                
                factory.CreateLogger("Handlebars")
                    .Log(logLevel, 0, arguments, null, (objects, exception) => format(arguments));
            };
        }
    }
}