namespace HandlebarsDotNet.Extension.Logger
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class LoggerFeatureExtensions
    {
        /// <summary>
        /// Adds <c>log</c> helper that uses provided <paramref name="logger"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static HandlebarsConfiguration UseLogger(this HandlebarsConfiguration configuration, Log logger)
        {
            var compileTimeConfiguration = configuration.CompileTimeConfiguration;
            
            compileTimeConfiguration.Features.Add(new LoggerFeatureFactory(logger));

            return configuration;
        }
    }
}