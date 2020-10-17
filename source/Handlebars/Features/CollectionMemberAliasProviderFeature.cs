using HandlebarsDotNet.MemberAliasProvider;

namespace HandlebarsDotNet.Features
{
    internal class CollectionMemberAliasProviderFeature : IFeature
    {
        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            var aliasProvider = new CollectionMemberAliasProvider(configuration);
            configuration.AliasProviders.Add(aliasProvider);
        }

        public void CompilationCompleted()
        {
            //nothing to do here
        }
    }
    
    internal class CollectionMemberAliasProviderFeatureFactory : IFeatureFactory
    {
        public IFeature CreateFeature() => new CollectionMemberAliasProviderFeature();
    }

    public static class CollectionMemberAliasProviderExtensions
    {
        /// <summary>
        /// Adds support for resolving `.length` from `.count` and vice versa
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static HandlebarsConfiguration UseCollectionMemberAliasProvider(this HandlebarsConfiguration configuration)
        {
            configuration.CompileTimeConfiguration.Features.Add(new CollectionMemberAliasProviderFeatureFactory());

            return configuration;
        }
    }
}