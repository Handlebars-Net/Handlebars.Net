using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Features;

namespace HandlebarsDotNet.Extension.Logger
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="level"></param>
    /// <param name="format"></param>
    public delegate void Log(object[] arguments, LoggingLevel level, Func<object[], string> format);
    
    internal class LoggerFeature : IFeature
    {
        private readonly Log _logger;
        
        private readonly Func<object[], string> _defaultFormatter = objects => string.Join("; ", objects);

        public LoggerFeature(Log logger)
        {
            _logger = logger;
        }
        
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            if (configuration.ReturnHelpers.TryGetValue("log", out var logger))
            {
                configuration.ReturnHelpers["log"] = (context, arguments) =>
                {
                    logger(context, arguments);
                    return LogHelper(context, arguments);
                };
            }
            else
            {
                configuration.ReturnHelpers["log"] = LogHelper;   
            }
        }

        public void CompilationCompleted()
        {
        }

        private string LogHelper(dynamic context, object[] arguments)
        {
            var logLevel = LoggingLevel.Info;
            var formatter = _defaultFormatter;

            var logArguments = arguments;
            if (arguments.Last() is IDictionary<string, object> hash)
            {
                logArguments = arguments.Take(arguments.Length - 1).ToArray();
                if(hash.TryGetValue("level", out var level))
                {
                    if(Enum.TryParse<LoggingLevel>(level.ToString(), true, out var hbLevel))
                    {
                        logLevel = hbLevel;
                    }
                }

                if (hash.TryGetValue("format", out var format))
                {
                    var formatString = format.ToString()
                        .Replace("[", "{")
                        .Replace("]", "}");
                    
                    formatter = objects => string.Format(formatString, objects);
                }
            }

            _logger(logArguments, logLevel, formatter);
            
            return string.Empty;
        }
    }
}