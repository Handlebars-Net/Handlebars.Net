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
    public partial class ClosureBuilder
    {
        private readonly List<KeyValuePair<ConstantExpression, PathInfo>> _pathInfos = new();
        private readonly List<KeyValuePair<ConstantExpression, TemplateDelegate>> _templateDelegates = new();
        private readonly List<KeyValuePair<ConstantExpression, DecoratorDelegate>> _decoratorDelegates = new();
        private readonly List<KeyValuePair<ConstantExpression, ChainSegment[]>> _blockParams = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<HelperOptions>>>> _helpers = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<BlockHelperOptions>>>> _blockHelpers = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<DecoratorOptions>>>> _decorators = new();
        private readonly List<KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>>> _blockDecorators = new();
        private readonly List<KeyValuePair<ConstantExpression, object>> _other = new();
        
        public void Add(ConstantExpression constantExpression)
        {
            if (constantExpression.Type == typeof(PathInfo))
            {
                _pathInfos.Add(new KeyValuePair<ConstantExpression, PathInfo>(constantExpression, (PathInfo) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(Ref<IHelperDescriptor<HelperOptions>>))
            {
                _helpers.Add(new KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<HelperOptions>>>(constantExpression, (Ref<IHelperDescriptor<HelperOptions>>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(Ref<IHelperDescriptor<BlockHelperOptions>>))
            {
                _blockHelpers.Add(new KeyValuePair<ConstantExpression, Ref<IHelperDescriptor<BlockHelperOptions>>>(constantExpression, (Ref<IHelperDescriptor<BlockHelperOptions>>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(Ref<IDecoratorDescriptor<DecoratorOptions>>))
            {
                _decorators.Add(new KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<DecoratorOptions>>>(constantExpression, (Ref<IDecoratorDescriptor<DecoratorOptions>>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(Ref<IDecoratorDescriptor<BlockDecoratorOptions>>))
            {
                _blockDecorators.Add(new KeyValuePair<ConstantExpression, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>>(constantExpression, (Ref<IDecoratorDescriptor<BlockDecoratorOptions>>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(TemplateDelegate))
            {
                _templateDelegates.Add(new KeyValuePair<ConstantExpression, TemplateDelegate>(constantExpression, (TemplateDelegate) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(DecoratorDelegate))
            {
                _decoratorDelegates.Add(new KeyValuePair<ConstantExpression, DecoratorDelegate>(constantExpression, (DecoratorDelegate) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(ChainSegment[]))
            {
                _blockParams.Add(new KeyValuePair<ConstantExpression, ChainSegment[]>(constantExpression, (ChainSegment[]) constantExpression.Value));
            }
            else
            {
                _other.Add(new KeyValuePair<ConstantExpression, object>(constantExpression, constantExpression.Value));
            }
        }

        public KeyValuePair<ParameterExpression, Dictionary<Expression, Expression>> Build(out Closure closure)
        {
            var closureType = typeof(Closure);
            var constructor = closureType
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single();
            
            var arguments = new List<object>();

            BuildKnownValues(arguments, _pathInfos, 4);
            BuildKnownValues(arguments, _helpers, 4);
            BuildKnownValues(arguments, _blockHelpers, 4);
            BuildKnownValues(arguments, _templateDelegates, 4);
            BuildKnownValues(arguments, _blockParams, 1);
            BuildKnownValues(arguments, _decorators, 4);
            BuildKnownValues(arguments, _blockDecorators, 4);
            BuildKnownValues(arguments, _decoratorDelegates, 4);
            arguments.Add(_other.Select(o => o.Value).ToArray());

            closure = (Closure) constructor.Invoke(arguments.ToArray());
            
            var mapping = new Dictionary<Expression, Expression>();

            var closureExpression = Expression.Variable(typeof(Closure), "closure");
            
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

        private static void BuildKnownValues<T>(List<object> arguments, List<KeyValuePair<ConstantExpression, T>> knowValues, int fieldsCount)
            where T: class
        {
            for (var index = 0; index < fieldsCount; index++)
            {
                arguments.Add(knowValues.ElementAtOrDefault(index).Value);
            }

            arguments.Add(knowValues.Count > fieldsCount ? knowValues.Skip(fieldsCount).Select(o => o.Value).ToArray() : null);
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
        public readonly PathInfo PI0;
        public readonly PathInfo PI1;
        public readonly PathInfo PI2;
        public readonly PathInfo PI3;
        public readonly PathInfo[] PIA;
        
        public readonly Ref<IHelperDescriptor<HelperOptions>> HD0;
        public readonly Ref<IHelperDescriptor<HelperOptions>> HD1;
        public readonly Ref<IHelperDescriptor<HelperOptions>> HD2;
        public readonly Ref<IHelperDescriptor<HelperOptions>> HD3;
        public readonly Ref<IHelperDescriptor<HelperOptions>>[] HDA;
        
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>> BHD0;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>> BHD1;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>> BHD2;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>> BHD3;
        public readonly Ref<IHelperDescriptor<BlockHelperOptions>>[] BHDA;
        
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>> DD0;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>> DD1;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>> DD2;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>> DD3;
        public readonly Ref<IDecoratorDescriptor<DecoratorOptions>>[] DDA;
        
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>> BDD0;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>> BDD1;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>> BDD2;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>> BDD3;
        public readonly Ref<IDecoratorDescriptor<BlockDecoratorOptions>>[] BDDA;
        
        public readonly TemplateDelegate TD0;
        public readonly TemplateDelegate TD1;
        public readonly TemplateDelegate TD2;
        public readonly TemplateDelegate TD3;
        public readonly TemplateDelegate[] TDA;
        
        public readonly DecoratorDelegate DDD0;
        public readonly DecoratorDelegate DDD1;
        public readonly DecoratorDelegate DDD2;
        public readonly DecoratorDelegate DDD3;
        public readonly DecoratorDelegate[] DDDA;
        
        public readonly ChainSegment[] BP0;
        public readonly ChainSegment[][] BPA;

        public readonly object[] A;

        internal Closure(PathInfo pi0, PathInfo pi1, PathInfo pi2, PathInfo pi3, PathInfo[] pia, Ref<IHelperDescriptor<HelperOptions>> hd0, Ref<IHelperDescriptor<HelperOptions>> hd1, Ref<IHelperDescriptor<HelperOptions>> hd2, Ref<IHelperDescriptor<HelperOptions>> hd3, Ref<IHelperDescriptor<HelperOptions>>[] hda, Ref<IHelperDescriptor<BlockHelperOptions>> bhd0, Ref<IHelperDescriptor<BlockHelperOptions>> bhd1, Ref<IHelperDescriptor<BlockHelperOptions>> bhd2, Ref<IHelperDescriptor<BlockHelperOptions>> bhd3, Ref<IHelperDescriptor<BlockHelperOptions>>[] bhda, TemplateDelegate td0, TemplateDelegate td1, TemplateDelegate td2, TemplateDelegate td3, TemplateDelegate[] tda, ChainSegment[] bp0, ChainSegment[][] bpa, Ref<IDecoratorDescriptor<DecoratorOptions>> dd0, Ref<IDecoratorDescriptor<DecoratorOptions>> dd1, Ref<IDecoratorDescriptor<DecoratorOptions>> dd2, Ref<IDecoratorDescriptor<DecoratorOptions>> dd3, Ref<IDecoratorDescriptor<DecoratorOptions>>[] dda, Ref<IDecoratorDescriptor<BlockDecoratorOptions>> bdd0, Ref<IDecoratorDescriptor<BlockDecoratorOptions>> bdd1, Ref<IDecoratorDescriptor<BlockDecoratorOptions>> bdd2, Ref<IDecoratorDescriptor<BlockDecoratorOptions>> bdd3, Ref<IDecoratorDescriptor<BlockDecoratorOptions>>[] bdda, DecoratorDelegate ddd0, DecoratorDelegate ddd1, DecoratorDelegate ddd2, DecoratorDelegate ddd3, DecoratorDelegate[] ddda, object[] a)
        {
            PI0 = pi0;
            PI1 = pi1;
            PI2 = pi2;
            PI3 = pi3;
            PIA = pia;
            HD0 = hd0;
            HD1 = hd1;
            HD2 = hd2;
            HD3 = hd3;
            HDA = hda;
            BHD0 = bhd0;
            BHD1 = bhd1;
            BHD2 = bhd2;
            BHD3 = bhd3;
            BHDA = bhda;
            TD0 = td0;
            TD1 = td1;
            TD2 = td2;
            TD3 = td3;
            TDA = tda;
            BP0 = bp0;
            BPA = bpa;
            DD0 = dd0;
            DD1 = dd1;
            DD2 = dd2;
            DD3 = dd3;
            DDA = dda;
            BDD0 = bdd0;
            BDD1 = bdd1;
            BDD2 = bdd2;
            BDD3 = bdd3;
            BDDA = bdda;
            DDD0 = ddd0;
            DDD1 = ddd1;
            DDD2 = ddd2;
            DDD3 = ddd3;
            DDDA = ddda;
            A = a;
        }
    }
}