using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Features
{
    /// <summary>
    /// Allows to warm-up internal caches for specific types
    /// </summary>
    public class WarmUpFeature : IFeature
    {
        private readonly HashSet<Type> _types;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        public WarmUpFeature(HashSet<Type> types)
        {
            _types = types;
        }

        /// <inheritdoc cref="IFeature.OnCompiling"/>
        public void OnCompiling(HandlebarsConfiguration configuration)
        {
            var internalConfiguration = (InternalHandlebarsConfiguration) configuration;
            
            var descriptorProvider = internalConfiguration.ObjectDescriptorProvider;
            foreach (var type in _types)
            {
                if(!descriptorProvider.CanHandleType(type)) continue;
                descriptorProvider.TryGetDescriptor(type, out _);
            }
        }

        /// <inheritdoc cref="IFeature.CompilationCompleted"/>
        public void CompilationCompleted()
        {
            // noting to do here
        }
    }

    internal class WarmUpFeatureFactory : IFeatureFactory
    {
        private readonly HashSet<Type> _types;

        public WarmUpFeatureFactory(HashSet<Type> types)
        {
            _types = types;
        }

        public IFeature CreateFeature()
        {
            return new WarmUpFeature(_types);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
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
}