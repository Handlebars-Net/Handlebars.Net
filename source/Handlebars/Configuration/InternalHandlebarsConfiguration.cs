using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet
{
    internal sealed class InternalHandlebarsConfiguration : HandlebarsConfiguration, ICompiledHandlebarsConfiguration
    {
        private readonly HandlebarsConfiguration _configuration;
        
        public override IExpressionNameResolver ExpressionNameResolver => _configuration.ExpressionNameResolver;
        public override ITextEncoder TextEncoder => _configuration.TextEncoder;
        public override IFormatProvider FormatProvider => _configuration.FormatProvider;
        public override ViewEngineFileSystem FileSystem => _configuration.FileSystem;
        public override string UnresolvedBindingFormatter => _configuration.UnresolvedBindingFormatter;
        public override bool ThrowOnUnresolvedBindingExpression => _configuration.ThrowOnUnresolvedBindingExpression;
        public override IPartialTemplateResolver PartialTemplateResolver => _configuration.PartialTemplateResolver;
        public override IMissingPartialTemplateHandler MissingPartialTemplateHandler => _configuration.MissingPartialTemplateHandler;
        public override Compatibility Compatibility => _configuration.Compatibility;
        public override CompileTimeConfiguration CompileTimeConfiguration { get; }
        
        public List<IFeature> Features { get; }
        public IObjectDescriptorProvider ObjectDescriptorProvider { get; }
        public ICollection<IExpressionMiddleware> ExpressionMiddleware => CompileTimeConfiguration.ExpressionMiddleware;
        public ICollection<IMemberAliasProvider> AliasProviders => CompileTimeConfiguration.AliasProviders;
        public IExpressionCompiler ExpressionCompiler
        {
            get => CompileTimeConfiguration.ExpressionCompiler;
            set => CompileTimeConfiguration.ExpressionCompiler = value;
        }
        
        public bool UseAggressiveCaching
        {
            get => CompileTimeConfiguration.UseAggressiveCaching;
            set => CompileTimeConfiguration.UseAggressiveCaching = value;
        }
        IReadOnlyList<IFeature> ICompiledHandlebarsConfiguration.Features => Features;

        public PathStore Paths { get; }

        internal InternalHandlebarsConfiguration(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
            
            Helpers = new CascadeDictionary<string, HandlebarsHelper>(configuration.Helpers, StringComparer.OrdinalIgnoreCase);
            ReturnHelpers = new CascadeDictionary<string, HandlebarsReturnHelper>(configuration.ReturnHelpers, StringComparer.OrdinalIgnoreCase);
            BlockHelpers = new CascadeDictionary<string, HandlebarsBlockHelper>(configuration.BlockHelpers, StringComparer.OrdinalIgnoreCase);
            RegisteredTemplates = new CascadeDictionary<string, Action<TextWriter, object>>(configuration.RegisteredTemplates, StringComparer.OrdinalIgnoreCase);
            HelperResolvers = new CascadeCollection<IHelperResolver>(configuration.HelperResolvers);
            Paths = new PathStore();
            
            CompileTimeConfiguration = new CompileTimeConfiguration
            {
                UseAggressiveCaching = _configuration.CompileTimeConfiguration.UseAggressiveCaching,
                ExpressionCompiler = _configuration.CompileTimeConfiguration.ExpressionCompiler,
                ExpressionMiddleware = new List<IExpressionMiddleware>(configuration.CompileTimeConfiguration.ExpressionMiddleware),
                Features = new List<IFeatureFactory>(configuration.CompileTimeConfiguration.Features),
                AliasProviders = new List<IMemberAliasProvider>(configuration.CompileTimeConfiguration.AliasProviders)
                {
                    new CollectionMemberAliasProvider(this)
                }
            };
            
            var objectDescriptorProvider = new ObjectDescriptorProvider(this);
            var listObjectDescriptor = new CollectionObjectDescriptor(objectDescriptorProvider);
            var providers = new List<IObjectDescriptorProvider>(configuration.CompileTimeConfiguration.ObjectDescriptorProviders)
            {
                new ContextObjectDescriptor(),
                new StringDictionaryObjectDescriptorProvider(),
                new GenericDictionaryObjectDescriptorProvider(),
                new DictionaryObjectDescriptor(),
                listObjectDescriptor,
                new EnumerableObjectDescriptor(listObjectDescriptor),
                new KeyValuePairObjectDescriptorProvider(),
                objectDescriptorProvider,
                new DynamicObjectDescriptor()
            };

            ObjectDescriptorProvider = new ObjectDescriptorFactory(providers);

            Features = CompileTimeConfiguration
                .Features.Select(o => o.CreateFeature())
                .OrderBy(o => o.GetType().GetTypeInfo().GetCustomAttribute<FeatureOrderAttribute>()?.Order ?? 100)
                .ToList();
        }
    }
}