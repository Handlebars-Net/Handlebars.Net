using System;
using Microsoft.Extensions.Logging;

namespace HandlebarsDotNet.Extension.Logger
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class LoggerFeatureExtensions
    {
        /// <summary>
        /// Adds <c>log</c> helper that uses provided <paramref name="loggerFactory"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static HandlebarsConfiguration UseLogger(this HandlebarsConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var compileTimeConfiguration = configuration.CompileTimeConfiguration;
            
            compileTimeConfiguration.Features.Add(new LoggerFeatureFactory(loggerFactory));

            return configuration;
        }
    }
}