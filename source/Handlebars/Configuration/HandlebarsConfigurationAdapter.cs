using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Middlewares;
using HandlebarsDotNet.Compiler.Resolvers;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    internal class HandlebarsConfigurationAdapter : ICompiledHandlebarsConfiguration
    {
        // observers are in WeakCollection, need to keep reference somewhere
        private readonly List<object> _observers = new List<object>();
        
        public HandlebarsConfigurationAdapter(HandlebarsConfiguration configuration)
        {
            UnderlingConfiguration = configuration;

            HelperResolvers = new ObservableList<IHelperResolver>(configuration.HelperResolvers);
            RegisteredTemplates = new ObservableIndex<string, HandlebarsTemplate<TextWriter, object, object>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.OrdinalIgnoreCase), configuration.RegisteredTemplates);
            AliasProviders = new ObservableList<IMemberAliasProvider>(configuration.AliasProviders);
            FormatterProviders = new ObservableList<IFormatterProvider>
            {
                new DefaultFormatterProvider(),
                new CollectionFormatterProvider(),
                new ReadOnlyCollectionFormatterProvider()
            }.AddMany(configuration.FormatterProviders);
            configuration.FormatterProviders.Subscribe(FormatterProviders);
            
            ObjectDescriptorProviders = CreateObjectDescriptorProvider(UnderlingConfiguration.ObjectDescriptorProviders);
            ExpressionMiddlewares = new ObservableList<IExpressionMiddleware>(configuration.CompileTimeConfiguration.ExpressionMiddleware)
            {
                new ClosureExpressionMiddleware(),
                new ExpressionOptimizerMiddleware()
            };

            Features = UnderlingConfiguration.CompileTimeConfiguration.Features
                .Select(o => o.CreateFeature())
                .OrderBy(o => o.GetType().GetTypeInfo().GetCustomAttribute<FeatureOrderAttribute>()?.Order ?? 100)
                .ToList();
            
            Helpers = CreateHelpersSubscription<IHelperDescriptor<HelperOptions>, HelperOptions>(configuration.Helpers);
            BlockHelpers = CreateHelpersSubscription<IHelperDescriptor<BlockHelperOptions>, BlockHelperOptions>(configuration.BlockHelpers);
            Decorators = CreateHelpersSubscription<IDecoratorDescriptor<DecoratorOptions>, DecoratorOptions>(configuration.Decorators);
            BlockDecorators = CreateHelpersSubscription<IDecoratorDescriptor<BlockDecoratorOptions>, BlockDecoratorOptions>(configuration.BlockDecorators);
        }

        public HandlebarsConfiguration UnderlingConfiguration { get; }
        public IExpressionNameResolver ExpressionNameResolver => UnderlingConfiguration.ExpressionNameResolver;
        public ITextEncoder TextEncoder => UnderlingConfiguration.TextEncoder;
        public IFormatProvider FormatProvider => UnderlingConfiguration.FormatProvider;
        public ViewEngineFileSystem FileSystem => UnderlingConfiguration.FileSystem;
        public ObservableList<IFormatterProvider> FormatterProviders { get; }
        public bool ThrowOnUnresolvedBindingExpression => UnderlingConfiguration.ThrowOnUnresolvedBindingExpression;
        public IPartialTemplateResolver PartialTemplateResolver => UnderlingConfiguration.PartialTemplateResolver;
        public IMissingPartialTemplateHandler MissingPartialTemplateHandler => UnderlingConfiguration.MissingPartialTemplateHandler;
        public Compatibility Compatibility => UnderlingConfiguration.Compatibility;

        public bool NoEscape => UnderlingConfiguration.NoEscape;
        
        public ObservableList<IObjectDescriptorProvider> ObjectDescriptorProviders { get; }
        public IAppendOnlyList<IExpressionMiddleware> ExpressionMiddlewares { get; }
        public IAppendOnlyList<IMemberAliasProvider> AliasProviders { get; }
        public IExpressionCompiler ExpressionCompiler { get; set; }
        public IReadOnlyList<IFeature> Features { get; }
        
        public IIndexed<PathInfoLight, Ref<IHelperDescriptor<HelperOptions>>> Helpers { get; }
        public IIndexed<PathInfoLight, Ref<IHelperDescriptor<BlockHelperOptions>>> BlockHelpers { get; }
        public IIndexed<PathInfoLight, Ref<IDecoratorDescriptor<DecoratorOptions>>> Decorators { get; }
        public IIndexed<PathInfoLight, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>> BlockDecorators { get; }
        public IAppendOnlyList<IHelperResolver> HelperResolvers { get; }
        public IIndexed<string, HandlebarsTemplate<TextWriter, object, object>> RegisteredTemplates { get; }
        
        private ObservableIndex<PathInfoLight, Ref<TDescriptor>, PathInfoLight.PathInfoLightEqualityComparer> CreateHelpersSubscription<TDescriptor, TOptions>(IIndexed<string, TDescriptor> source) 
            where TOptions : struct, IOptions
            where TDescriptor : class, IDescriptor<TOptions>
        {
            var equalityComparer = Compatibility.RelaxedHelperNaming ? PathInfoLight.PlainPathComparer : PathInfoLight.PlainPathWithPartsCountComparer;
            var existingHelpers = source.ToIndexed(
                o => (PathInfoLight) $"[{o.Key}]", 
                o => new Ref<TDescriptor>(o.Value),
                equalityComparer
            );
            
            var target = new ObservableIndex<PathInfoLight, Ref<TDescriptor>, PathInfoLight.PathInfoLightEqualityComparer>(equalityComparer, existingHelpers);

            var observer = ObserverBuilder<ObservableEvent<TDescriptor>>.Create(target)
                .OnEvent<DictionaryAddedObservableEvent<string, TDescriptor>>(
                    (@event, state) =>
                    {
                        PathInfoLight key = $"[{@event.Key}]";
                        if (state.TryGetValue(key, out var @ref))
                        {
                            @ref.Value = @event.Value;
                            return;
                        }
                        
                        state.AddOrReplace(key, new Ref<TDescriptor>(@event.Value));
                    })
                .Build();

            _observers.Add(observer);
            
            source.As<ObservableIndex<string, TDescriptor, StringEqualityComparer>>()?.Subscribe(observer);

            return target;
        }
        
        private ObservableList<IObjectDescriptorProvider> CreateObjectDescriptorProvider(ObservableList<IObjectDescriptorProvider> descriptorProviders)
        {
            var objectDescriptorProvider = new ObjectDescriptorProvider(AliasProviders);
            var objectDescriptorProviders = new ObservableList<IObjectDescriptorProvider>
                {
                    objectDescriptorProvider,
                    new DynamicObjectDescriptor(objectDescriptorProvider),
                    new EnumerableObjectDescriptor(objectDescriptorProvider),
                    new DictionaryObjectDescriptor(),
                    new ReadOnlyGenericDictionaryObjectDescriptorProvider(),
                    new GenericDictionaryObjectDescriptorProvider(),
                    new ReadOnlyStringDictionaryObjectDescriptorProvider(),
                    new StringDictionaryObjectDescriptorProvider(),
                    new LayoutViewModel.DescriptorProvider(),
                }
                .AddMany(descriptorProviders);

            descriptorProviders.Subscribe(objectDescriptorProviders);

            return objectDescriptorProviders;
        }
    }
}
