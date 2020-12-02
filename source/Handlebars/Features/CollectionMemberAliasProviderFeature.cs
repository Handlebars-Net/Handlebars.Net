using HandlebarsDotNet.MemberAliasProvider;

namespace HandlebarsDotNet.Features
{
    internal class CollectionMemberAliasProviderFeature : IFeature
    {
        private static readonly CollectionMemberAliasProvider AliasProvider = new CollectionMemberAliasProvider();

        public void OnCompiling(ICompiledHandlebarsConfiguration configuration)
        {
            configuration.AliasProviders.Add(AliasProvider);
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