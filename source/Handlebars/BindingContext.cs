using System;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    public sealed partial class BindingContext : IDisposable
    {
        internal readonly EntryIndex<ChainSegment>[] WellKnownVariables = new EntryIndex<ChainSegment>[8];
        
        internal readonly DeferredValue<BindingContext, ObjectDescriptor> ObjectDescriptor;

        private BindingContext()
        {
            InlinePartialTemplates = new CascadeIndex<string, Action<EncodedTextWriter, BindingContext>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.OrdinalIgnoreCase));
            
            Bag = new CascadeIndex<string, object, StringEqualityComparer>(new StringEqualityComparer(StringComparison.OrdinalIgnoreCase));
            RootDataObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            ContextDataObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            BlockParamsObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            
            ObjectDescriptor = new DeferredValue<BindingContext, ObjectDescriptor>(this, context =>
            {
                return ObjectDescriptors.ObjectDescriptor.Create(context.Value, context.Configuration.ObjectDescriptorProvider);
            });
        }
        
        internal CascadeIndex<string, object, StringEqualityComparer> Bag { get; }
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> RootDataObject { get; }
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> ContextDataObject { get; }
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> BlockParamsObject { get; }

        internal void SetDataObject(object data)
        {
            if(data == null) return;

            if (!Configuration.ObjectDescriptorProvider.TryGetDescriptor(data.GetType(), out var objectDescriptor))
            {
                throw new HandlebarsRuntimeException($"Cannot resolve object descriptor for type `{data.GetType()}`");
            }
            
            var objectAccessor = new ObjectAccessor(data, objectDescriptor);

            foreach (var property in objectAccessor.Properties)
            {
                var value = objectAccessor[property];
                RootDataObject.AddOrReplace(property, value, out _);
                ContextDataObject.AddOrReplace(property, value, out _);
            }
        }
        
        private void Initialize()
        {
            Root = ParentContext?.Root ?? this;
            
            if(!ReferenceEquals(Root, this)) Root.RootDataObject.CopyTo(ContextDataObject);
            ContextDataObject.AddOrReplace(ChainSegment.Root, Root.Value, out WellKnownVariables[(int) WellKnownVariable.Root]);

            if (ParentContext == null)
            {
                ContextDataObject.AddOrReplace(
                    ChainSegment.Parent, 
                    UndefinedBindingResult.Create(ChainSegment.Parent), 
                    out WellKnownVariables[(int) WellKnownVariable.Parent]
                );
                
                return;
            }

            ContextDataObject.AddOrReplace(
                ChainSegment.Parent, 
                ParentContext.Value, 
                out WellKnownVariables[(int) WellKnownVariable.Parent]
            );

            Bag.Outer = ParentContext.Bag;
            ParentContext.BlockParamsObject.CopyTo(BlockParamsObject);

            //Inline partials cannot use the Handlebars.RegisteredTemplate method
            //because it pollutes the static dictionary and creates collisions
            //where the same partial name might exist in multiple templates.
            //To avoid collisions, pass around a dictionary of compiled partials
            //in the context
            InlinePartialTemplates.Outer = ParentContext.InlinePartialTemplates;

            if (!(Value is HashParameterDictionary dictionary) || ParentContext.Value == null || ReferenceEquals(Value, ParentContext.Value)) return;
            
            // Populate value with parent context
            PopulateHash(dictionary, ParentContext.Value, Configuration);
        }

        internal ICompiledHandlebarsConfiguration Configuration { get; private set; }
        
        internal CascadeIndex<string, Action<EncodedTextWriter, BindingContext>, StringEqualityComparer> InlinePartialTemplates { get; }

        internal TemplateDelegate PartialBlockTemplate { get; private set; }
        
        public object Value { get; set; }

        /// <summary>
        /// Used to cary additional data
        /// </summary>
        public IIndexed<string, object> Extensions => Bag;

        internal BindingContext ParentContext { get; private set; }

        internal BindingContext Root { get; private set; }

        internal bool TryGetVariable(ChainSegment segment, out object value)
        {
            if (segment.WellKnownVariable != WellKnownVariable.None)
            {
                var wellKnownVariable = WellKnownVariables[(int) segment.WellKnownVariable];
                return BlockParamsObject.TryGetValue(wellKnownVariable, out value) 
                       || (ObjectDescriptor.Value?.MemberAccessor.TryGetValue(Value, segment, out value) ?? false);
            }
            
            return BlockParamsObject.TryGetValue(segment, out value) 
                   || (ObjectDescriptor.Value?.MemberAccessor.TryGetValue(Value, segment, out value) ?? false);
        }
        
        internal bool TryGetContextVariable(ChainSegment segment, out object value)
        {
            if (segment.WellKnownVariable != WellKnownVariable.None)
            {
                var wellKnownVariable = WellKnownVariables[(int) segment.WellKnownVariable];
                return BlockParamsObject.TryGetValue(segment, out value)
                   || ContextDataObject.TryGetValue(wellKnownVariable, out value);
            }
            
            return BlockParamsObject.TryGetValue(segment, out value)
                   || ContextDataObject.TryGetValue(segment, out value);
        }

        internal BindingContext CreateChildContext(object value, TemplateDelegate partialBlockTemplate = null)
        {
            return Create(Configuration, value ?? Value, this, partialBlockTemplate ?? PartialBlockTemplate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(object value = null)
        {
            return Create(Configuration, value, this, PartialBlockTemplate);
        }

        private static void PopulateHash(HashParameterDictionary hash, object from, ICompiledHandlebarsConfiguration configuration)
        {
            var descriptor = HandlebarsDotNet.ObjectDescriptors.ObjectDescriptor.Create(from, configuration.ObjectDescriptorProvider);
            var accessor = descriptor.MemberAccessor;
            var properties = descriptor.GetProperties(descriptor, from);
            var enumerator = properties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var segment = ChainSegment.Create(enumerator.Current);
                if(hash.ContainsKey(segment)) continue;
                if (!accessor.TryGetValue(@from, segment, out var value)) continue;
                hash[segment] = value;
            }
        }
    }
}
