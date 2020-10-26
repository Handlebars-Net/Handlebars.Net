using System;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ObjectDescriptors;

namespace HandlebarsDotNet.Compiler
{
    public sealed partial class BindingContext : IDisposable
    {
        internal readonly EntryIndex<ChainSegment>[] WellKnownVariables = new EntryIndex<ChainSegment>[8];
        
        private readonly DeferredValue<BindingContext, ObjectDescriptor> _objectDescriptor;

        private BindingContext()
        {
            InlinePartialTemplates = new CascadeDictionary<string, Action<EncodedTextWriter, BindingContext>>();
            
            RootDataObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            ContextDataObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            BlockParamsObject = new FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer>(16, 7, ChainSegment.EqualityComparer);
            
            _objectDescriptor = new DeferredValue<BindingContext, ObjectDescriptor>(this, context => ObjectDescriptor.Create(context.Value, context.Configuration));
        }
        
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> RootDataObject { get; }
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> ContextDataObject { get; }
        internal FixedSizeDictionary<ChainSegment, object, ChainSegment.ChainSegmentEqualityComparer> BlockParamsObject { get; }

        internal void SetDataObject(object data)
        {
            if(data == null) return;
            
            var objectDescriptor = ObjectDescriptor.Create(data, Configuration);
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
            
            ParentContext.BlockParamsObject.CopyTo(BlockParamsObject);
            
            TemplatePath = ParentContext.TemplatePath ?? TemplatePath;

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

        internal FixedSizeDictionary<string, object, StringComparer> Extensions { get; } = new FixedSizeDictionary<string, object, StringComparer>(8, 7, StringComparer.OrdinalIgnoreCase);

        internal string TemplatePath { get; private set; }

        internal ICompiledHandlebarsConfiguration Configuration { get; private set; }
        
        internal CascadeDictionary<string, Action<EncodedTextWriter, BindingContext>> InlinePartialTemplates { get; }

        internal TemplateDelegate PartialBlockTemplate { get; private set; }
        
        public object Value { get; set; }

        internal BindingContext ParentContext { get; private set; }

        internal BindingContext Root { get; private set; }

        internal bool TryGetVariable(ChainSegment segment, out object value)
        {
            if (segment.WellKnownVariable != WellKnownVariable.None)
            {
                var wellKnownVariable = WellKnownVariables[(int) segment.WellKnownVariable];
                return BlockParamsObject.TryGetValue(wellKnownVariable, out value) 
                       || (_objectDescriptor.Value?.MemberAccessor.TryGetValue(Value, segment, out value) ?? false);
            }
            
            return BlockParamsObject.TryGetValue(segment, out value) 
                   || (_objectDescriptor.Value?.MemberAccessor.TryGetValue(Value, segment, out value) ?? false);
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
            return Create(Configuration, value ?? Value, this, TemplatePath, partialBlockTemplate ?? PartialBlockTemplate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(object value = null)
        {
            return Create(Configuration, value, this, TemplatePath, PartialBlockTemplate);
        }

        private static void PopulateHash(HashParameterDictionary hash, object from, ICompiledHandlebarsConfiguration configuration)
        {
            var descriptor = ObjectDescriptor.Create(from, configuration);
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
