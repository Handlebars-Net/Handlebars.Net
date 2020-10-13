using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Features
{
    public static class WarmUpFeatureExtensions
    {
        /// <summary>
        /// Allows to warm-up internal caches for specific types
        /// </summary>
        public static HandlebarsConfiguration UseWarmUp(this HandlebarsConfiguration configuration, Action<ICollection<Type>> configure)
        {
            var types = new HashSet<Type>();

            configure(types);
            
            configuration.CompileTimeConfiguration.Features.Add(new WarmUpFeatureFactory(types));
            
            return configuration;
        }
    }
    
    internal class WarmUpFeature : IFeature
    {
        private readonly HashSet<Type> _types;
        
        public WarmUpFeature(HashSet<Type> types) => _types = types;

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var descriptorProvider = configuration.ObjectDescriptorProvider;
            foreach (var type in _types)
            {
                descriptorProvider.TryGetDescriptor(type, out _);
            }
        }
        
        public void CompilationCompleted()
        {
            // noting to do here
        }
    }

    internal class WarmUpFeatureFactory : IFeatureFactory
    {
        private readonly HashSet<Type> _types;

        public WarmUpFeatureFactory(HashSet<Type> types) => _types = types;

        public IFeature CreateFeature() => new WarmUpFeature(_types);
    }
}