using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.Compiler
{
    public sealed partial class ClosureBuilder
    {
        private readonly List<KeyValuePair<ConstantExpression, PathInfo>> _pathInfos = new();
        private readonly List<KeyValuePair<ConstantExpression, TemplateDelegate>> _templateDelegates = new();
        private readonly List<KeyValuePair<ConstantExpression, DecoratorDelegate>> _decoratorDelegates = new();
        private readonly List<KeyValuePair<ConstantExpression, ChainSegment[]>> _blockParams = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<HelperOptions>>>> _helpers = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<BlockHelperOptions>>>> _blockHelpers = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<DecoratorOptions>>>> _decorators = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>>> _blockDecorators = new();
        private readonly List<KeyValuePair<ConstantExpression, object?>> _other = new();
        
        public void Add(ConstantExpression constantExpression)
        {
            if (constantExpression.Type == typeof(PathInfo))
            {
                _pathInfos.Add(new KeyValuePair<ConstantExpression, PathInfo>(constantExpression, (PathInfo) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(Ref<IHelperDescriptor<HelperOptions>>))
            {
                _helpers.Add(new KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<HelperOptions>>>(constantExpression, (Ref<IHelperDescriptor<HelperOptions>>) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(Ref<IHelperDescriptor<BlockHelperOptions>>))
            {
                _blockHelpers.Add(new KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<BlockHelperOptions>>>(constantExpression, (Ref<IHelperDescriptor<BlockHelperOptions>>) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(Ref<IDecoratorDescriptor<DecoratorOptions>>))
            {
                _decorators.Add(new KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<DecoratorOptions>>>(constantExpression, (Ref<IDecoratorDescriptor<DecoratorOptions>>) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(Ref<IDecoratorDescriptor<BlockDecoratorOptions>>))
            {
                _blockDecorators.Add(new KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>>(constantExpression, (Ref<IDecoratorDescriptor<BlockDecoratorOptions>>) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(TemplateDelegate))
            {
                _templateDelegates.Add(new KeyValuePair<ConstantExpression, TemplateDelegate>(constantExpression, (TemplateDelegate) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(DecoratorDelegate))
            {
                _decoratorDelegates.Add(new KeyValuePair<ConstantExpression, DecoratorDelegate>(constantExpression, (DecoratorDelegate) constantExpression.Value!));
            }
            else if (constantExpression.Type == typeof(ChainSegment[]))
            {
                _blockParams.Add(new KeyValuePair<ConstantExpression, ChainSegment[]>(constantExpression, (ChainSegment[]) constantExpression.Value!));
            }
            else
            {
                _other.Add(new KeyValuePair<ConstantExpression, object?>(constantExpression, constantExpression.Value));
            }
        }

        public KeyValuePair<ParameterExpression, Dictionary<Expression, Expression>> Build(out Closure closure)
        {
            closure = new Closure(_pathInfos, _helpers, _blockHelpers, _templateDelegates, _blockParams, _decorators,
                _blockDecorators, _decoratorDelegates, _other);
            
            var mapping = new Dictionary<Expression, Expression>();

            var closureType = typeof(Closure);
            var closureExpression = Expression.Variable(closureType, "closure");
            
            BuildKnownValuesExpressions(closureExpression, mapping, _pathInfos, "PI", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _helpers, "HD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _blockHelpers, "BHD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _templateDelegates, "TD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _blockParams, "BP", 1);
            BuildKnownValuesExpressions(closureExpression, mapping, _decorators, "DD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _blockDecorators, "BDD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _decoratorDelegates, "DDD", 4);
            
            var arrayField = closureType.GetField("A");
            var array = Expression.Field(closureExpression, arrayField!);
            for (int index = 0; index < _other.Count; index++)
            {
                var indexExpression = Expression.ArrayAccess(array, Expression.Constant(index));
                mapping.Add(_other[index].Key, indexExpression);
            }
            
            return new KeyValuePair<ParameterExpression, Dictionary<Expression, Expression>>(closureExpression, mapping);
        }

        private static void BuildKnownValuesExpressions<T>(Expression closure, Dictionary<Expression, Expression> expressions, List<KeyValuePair<ConstantExpression, T>> knowValues, string prefix, int fieldsCount)
            where T: class
        {
            var type = typeof(Closure);
            for (var index = 0; index < fieldsCount && index < knowValues.Count; index++)
            {
                var field = type.GetField($"{prefix}{index}");
                expressions.Add(knowValues[index].Key, Expression.Field(closure, field!));
            }

            var arrayField = type.GetField($"{prefix}A");
            var array = Expression.Field(closure, arrayField!);
            for (int index = fieldsCount, arrayIndex = 0; index < knowValues.Count; index++, arrayIndex++)
            {
                var indexExpression = Expression.ArrayAccess(array, Expression.Constant(arrayIndex));
                expressions.Add(knowValues[index].Key, indexExpression);
            }
        }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class Closure
    {
        public readonly PathInfo? PI0;
        public readonly PathInfo? PI1;
        public readonly PathInfo? PI2;
        public readonly PathInfo? PI3;
        public readonly PathInfo[]? PIA;
        
        public readonly Ref<IHelperDescriptor<HelperOptions>>? HD0;
        public readonly Ref<IHelperDescriptor<HelperOptions>>? HD1;
        public readonly Ref<IHelperDescriptor<HelperOptions>>? HD2;
        public readonly Ref<IHelperDescriptor<HelperOptions>>? HD3;
        public readonly Ref<IHelperDescriptor<HelperOptions>>[]? HDA;
        
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>? BHD0;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>? BHD1;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>? BHD2;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>? BHD3;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>[]? BHDA;
        
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>? DD0;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>? DD1;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>? DD2;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>? DD3;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>[]? DDA;
        
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>? BDD0;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>? BDD1;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>? BDD2;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>? BDD3;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>[]? BDDA;
        
        public readonly TemplateDelegate? TD0;
        public readonly TemplateDelegate? TD1;
        public readonly TemplateDelegate? TD2;
        public readonly TemplateDelegate? TD3;
        public readonly TemplateDelegate[]? TDA;
        
        public readonly DecoratorDelegate? DDD0;
        public readonly DecoratorDelegate? DDD1;
        public readonly DecoratorDelegate? DDD2;
        public readonly DecoratorDelegate? DDD3;
        public readonly DecoratorDelegate[]? DDDA;
        
        public readonly ChainSegment[]? BP0;
        public readonly ChainSegment[][]? BPA;

        public readonly object?[] A;

        internal Closure(
            List<KeyValuePair<ConstantExpression, PathInfo>> pathInfos,
            List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<HelperOptions>>>> helpers,
            List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<BlockHelperOptions>>>> blockHelpers,
            List<KeyValuePair<ConstantExpression, TemplateDelegate>> templateDelegates,
            List<KeyValuePair<ConstantExpression, ChainSegment[]>> blockParams,
            List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<DecoratorOptions>>>> decorators,
            List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>>> blockDecorators,
            List<KeyValuePair<ConstantExpression, DecoratorDelegate>> decoratorDelegates,
            List<KeyValuePair<ConstantExpression, object?>> other)
        {
            AssignValues(pathInfos, out PI0, out PI1, out PI2, out PI3, out PIA);
            AssignValues(helpers, out HD0, out HD1, out HD2, out HD3, out HDA);
            AssignValues(blockHelpers, out BHD0, out BHD1, out BHD2, out BHD3, out BHDA);
            AssignValues(templateDelegates, out TD0, out TD1, out TD2, out TD3, out TDA);
            AssignValues(blockParams, out BP0, out BPA);
            AssignValues(decorators, out DD0, out DD1, out DD2, out DD3, out DDA);
            AssignValues(blockDecorators, out BDD0, out BDD1, out BDD2, out BDD3, out BDDA);
            AssignValues(decoratorDelegates, out DDD0, out DDD1, out DDD2, out DDD3, out DDDA);
            A = other.Select(o => o.Value).ToArray();
        }

        private static void AssignValues<T>(List<KeyValuePair<ConstantExpression, T>> values, out T? v0, out T[]? a)
        {
            v0 = values.Count > 0 ? values[0].Value : default;
            a = values.Count > 1 ? values.Skip(1).Select(o => o.Value).ToArray() : null;
        }

        private static void AssignValues<T>(List<KeyValuePair<ConstantExpression, T>> values, out T? v0, out T? v1,
            out T? v2, out T? v3, out T[]? a)
        {
            v0 = values.Count > 0 ? values[0].Value : default;
            v1 = values.Count > 1 ? values[1].Value : default;
            v2 = values.Count > 2 ? values[2].Value : default;
            v3 = values.Count > 3 ? values[3].Value : default;
            a = values.Count > 4 ? values.Skip(4).Select(o => o.Value).ToArray() : null;
        }
    }
}