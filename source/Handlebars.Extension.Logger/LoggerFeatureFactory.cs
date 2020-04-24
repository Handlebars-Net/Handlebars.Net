using HandlebarsDotNet.Features;

namespace HandlebarsDotNet.Extension.Logger
{
    internal partial class LoggerFeatureFactory : IFeatureFactory
    {
        private readonly Log _logger;

        public LoggerFeatureFactory(Log logger)
        {
            _logger = logger;
        }
        
        public IFeature CreateFeature()
        {
            return new LoggerFeature(_logger);
        }
    }
}