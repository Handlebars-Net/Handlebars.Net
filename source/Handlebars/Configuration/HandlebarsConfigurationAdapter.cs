using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Middlewares;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    internal class HandlebarsConfigurationAdapter : ICompiledHandlebarsConfiguration
    {
        private readonly PathInfoStore _pathInfoStore;
        
        public HandlebarsConfigurationAdapter(HandlebarsConfiguration configuration)
        {
            UnderlingConfiguration = configuration;

            HelperResolvers = new ObservableList<IHelperResolver>(configuration.HelperResolvers);
            RegisteredTemplates = new ObservableDictionary<string, HandlebarsTemplate<TextWriter, object, object>>(configuration.RegisteredTemplates);
            PathInfoStore = _pathInfoStore = HandlebarsDotNet.PathInfoStore.Shared;
            ObjectDescriptorProvider = CreateObjectDescriptorProvider();
            AliasProviders = new ObservableList<IMemberAliasProvider>(UnderlingConfiguration.AliasProviders);

            ExpressionMiddlewares = new ObservableList<IExpressionMiddleware>(UnderlingConfiguration.CompileTimeConfiguration.ExpressionMiddleware)
            {
                new ExpressionOptimizerMiddleware()
            };

            Features = UnderlingConfiguration.CompileTimeConfiguration.Features
                .Select(o => o.CreateFeature())
                .OrderBy(o => o.GetType().GetTypeInfo().GetCustomAttribute<FeatureOrderAttribute>()?.Order ?? 100)
                .ToList();
            
            CreateHelpersSubscription();
            CreateBlockHelpersSubscription();
        }

        public HandlebarsConfiguration UnderlingConfiguration { get; }
        public IExpressionNameResolver ExpressionNameResolver => UnderlingConfiguration.ExpressionNameResolver;
        public ITextEncoder TextEncoder => UnderlingConfiguration.TextEncoder;
        public IFormatProvider FormatProvider => UnderlingConfiguration.FormatProvider;
        public ViewEngineFileSystem FileSystem => UnderlingConfiguration.FileSystem;
        public Formatter<UndefinedBindingResult> UnresolvedBindingFormatter => UnderlingConfiguration.UnresolvedBindingFormatter;
        public bool ThrowOnUnresolvedBindingExpression => UnderlingConfiguration.ThrowOnUnresolvedBindingExpression;
        public IPartialTemplateResolver PartialTemplateResolver => UnderlingConfiguration.PartialTemplateResolver;
        public IMissingPartialTemplateHandler MissingPartialTemplateHandler => UnderlingConfiguration.MissingPartialTemplateHandler;
        public Compatibility Compatibility => UnderlingConfiguration.Compatibility;
        public bool NoEscape => UnderlingConfiguration.NoEscape;
        
        public IObjectDescriptorProvider ObjectDescriptorProvider { get; }
        public IList<IExpressionMiddleware> ExpressionMiddlewares { get; }
        public IList<IMemberAliasProvider> AliasProviders { get; }
        public IExpressionCompiler ExpressionCompiler { get; set; }
        public IReadOnlyList<IFeature> Features { get; }
        public IPathInfoStore PathInfoStore { get; }
        
        public IDictionary<PathInfoLight, Ref<IHelperDescriptor<HelperOptions>>> Helpers { get; private set; }
        public IDictionary<PathInfoLight, Ref<IHelperDescriptor<BlockHelperOptions>>> BlockHelpers { get; private set; }
        public IList<IHelperResolver> HelperResolvers { get; }
        public IDictionary<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        
        private void CreateHelpersSubscription()
        {
            var existingHelpers = UnderlingConfiguration.Helpers.ToDictionary(
                o => new PathInfoLight(_pathInfoStore.GetOrAdd($"[{o.Key}]")), 
                o => new Ref<IHelperDescriptor<HelperOptions>>(o.Value)
            );

            Helpers = new ObservableDictionary<PathInfoLight, Ref<IHelperDescriptor<HelperOptions>>>(existingHelpers, Compatibility.RelaxedHelperNaming ? PathInfoLight.PlainPathComparer : PathInfoLight.PlainPathWithPartsCountComparer);
            
            var helpersObserver = new ObserverBuilder<ObservableEvent<IHelperDescriptor<HelperOptions>>>()
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<HelperOptions>>.ReplacedObservableEvent>(
                    @event => Helpers[_pathInfoStore.GetOrAdd($"[{@event.Key}]")].Value = @event.Value
                    )
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<HelperOptions>>.AddedObservableEvent>(
                    @event =>
                    {
                        Helpers.AddOrUpdate(_pathInfoStore.GetOrAdd($"[{@event.Key}]"), 
                            h => new Ref<IHelperDescriptor<HelperOptions>>(h), 
                            (h, o) => o.Value = h, 
                            @event.Value);
                    })
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<HelperOptions>>.RemovedObservableEvent>(@event =>
                {
                    if (Helpers.TryGetValue(_pathInfoStore.GetOrAdd($"[{@event.Key}]"), out var helperToRemove))
                    {
                        helperToRemove.Value = new LateBindHelperDescriptor(@event.Key);
                    }
                })
                .Build();

            UnderlingConfiguration.Helpers
                .As<ObservableDictionary<string, IHelperDescriptor<HelperOptions>>>()
                .Subscribe(helpersObserver);
        }

        private void CreateBlockHelpersSubscription()
        {
            var existingBlockHelpers = UnderlingConfiguration.BlockHelpers.ToDictionary(
                o => (PathInfoLight)_pathInfoStore.GetOrAdd($"[{o.Key}]"),
                o => new Ref<IHelperDescriptor<BlockHelperOptions>>(o.Value)
            );

            BlockHelpers = new ObservableDictionary<PathInfoLight, Ref<IHelperDescriptor<BlockHelperOptions>>>(existingBlockHelpers, Compatibility.RelaxedHelperNaming ? PathInfoLight.PlainPathComparer : PathInfoLight.PlainPathWithPartsCountComparer);

            var blockHelpersObserver = new ObserverBuilder<ObservableEvent<IHelperDescriptor<BlockHelperOptions>>>()
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<BlockHelperOptions>>.ReplacedObservableEvent>(
                    @event => BlockHelpers[_pathInfoStore.GetOrAdd($"[{@event.Key}]")].Value = @event.Value)
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<BlockHelperOptions>>.AddedObservableEvent>(
                    @event =>
                    {
                        BlockHelpers.AddOrUpdate(_pathInfoStore.GetOrAdd($"[{@event.Key}]"), 
                            h => new Ref<IHelperDescriptor<BlockHelperOptions>>(h), 
                            (h, o) => o.Value = h, 
                            @event.Value);
                    })
                .OnEvent<ObservableDictionary<string, IHelperDescriptor<BlockHelperOptions>>.RemovedObservableEvent>(@event =>
                {
                    if (BlockHelpers.TryGetValue(_pathInfoStore.GetOrAdd($"[{@event.Key}]"), out var helperToRemove))
                    {
                        helperToRemove.Value = new LateBindBlockHelperDescriptor(@event.Key);
                    }
                })
                .Build();

            UnderlingConfiguration.BlockHelpers
                .As<ObservableDictionary<string, IHelperDescriptor<BlockHelperOptions>>>()
                .Subscribe(blockHelpersObserver);
        }

        private IObjectDescriptorProvider CreateObjectDescriptorProvider()
        {
            var objectDescriptorProvider = new ObjectDescriptorProvider(this);
            var providers = new List<IObjectDescriptorProvider>(UnderlingConfiguration.CompileTimeConfiguration.ObjectDescriptorProviders)
            {
                new StringDictionaryObjectDescriptorProvider(),
                new ReadOnlyStringDictionaryObjectDescriptorProvider(),
                new GenericDictionaryObjectDescriptorProvider(),
                new ReadOnlyGenericDictionaryObjectDescriptorProvider(),
                new DictionaryObjectDescriptor(),
                new EnumerableObjectDescriptor(objectDescriptorProvider),
                objectDescriptorProvider,
                new DynamicObjectDescriptor()
            };

            return new ObjectDescriptorFactory(providers);
        }
    }
}