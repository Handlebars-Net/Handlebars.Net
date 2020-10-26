using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

namespace HandlebarsDotNet.Compiler
{
    public class ClosureBuilder
    {
        private readonly List<KeyValuePair<ConstantExpression, PathInfo>> _pathInfos = new List<KeyValuePair<ConstantExpression, PathInfo>>();
        private readonly List<KeyValuePair<ConstantExpression, TemplateDelegate>> _templateDelegates = new List<KeyValuePair<ConstantExpression, TemplateDelegate>>();
        private readonly List<KeyValuePair<ConstantExpression, ChainSegment[]>> _blockParams = new List<KeyValuePair<ConstantExpression, ChainSegment[]>>();
        private readonly List<KeyValuePair<ConstantExpression, StrongBox<HelperDescriptorBase>>> _helpers = new List<KeyValuePair<ConstantExpression, StrongBox<HelperDescriptorBase>>>();
        private readonly List<KeyValuePair<ConstantExpression, StrongBox<BlockHelperDescriptorBase>>> _blockHelpers = new List<KeyValuePair<ConstantExpression, StrongBox<BlockHelperDescriptorBase>>>();
        private readonly List<KeyValuePair<ConstantExpression, object>> _other = new List<KeyValuePair<ConstantExpression, object>>();
        
        public void Add(ConstantExpression constantExpression)
        {
            if (constantExpression.Type == typeof(PathInfo))
            {
                _pathInfos.Add(new KeyValuePair<ConstantExpression, PathInfo>(constantExpression, (PathInfo) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(StrongBox<HelperDescriptorBase>))
            {
                _helpers.Add(new KeyValuePair<ConstantExpression, StrongBox<HelperDescriptorBase>>(constantExpression, (StrongBox<HelperDescriptorBase>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(StrongBox<BlockHelperDescriptorBase>))
            {
                _blockHelpers.Add(new KeyValuePair<ConstantExpression, StrongBox<BlockHelperDescriptorBase>>(constantExpression, (StrongBox<BlockHelperDescriptorBase>) constantExpression.Value));
            }
            else if (constantExpression.Type == typeof(TemplateDelegate))
            {
                _templateDelegates.Add(new KeyValuePair<ConstantExpression, TemplateDelegate>(constantExpression, (TemplateDelegate) constantExpression.Value));
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
            arguments.Add(_other.Select(o => o.Value).ToArray());

            closure = (Closure) constructor.Invoke(arguments.ToArray());
            
            var mapping = new Dictionary<Expression, Expression>();

            var closureExpression = Expression.Variable(typeof(Closure), "closure");
            
            BuildKnownValuesExpressions(closureExpression, mapping, _pathInfos, "PI", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _helpers, "HD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _blockHelpers, "BHD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _templateDelegates, "TD", 4);
            BuildKnownValuesExpressions(closureExpression, mapping, _blockParams, "BP", 1);
            
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
        
        public readonly StrongBox<HelperDescriptorBase> HD0;
        public readonly StrongBox<HelperDescriptorBase> HD1;
        public readonly StrongBox<HelperDescriptorBase> HD2;
        public readonly StrongBox<HelperDescriptorBase> HD3;
        public readonly StrongBox<HelperDescriptorBase>[] HDA;
        
        public readonly StrongBox<BlockHelperDescriptorBase> BHD0;
        public readonly StrongBox<BlockHelperDescriptorBase> BHD1;
        public readonly StrongBox<BlockHelperDescriptorBase> BHD2;
        public readonly StrongBox<BlockHelperDescriptorBase> BHD3;
        public readonly StrongBox<BlockHelperDescriptorBase>[] BHDA;
        
        public readonly TemplateDelegate TD0;
        public readonly TemplateDelegate TD1;
        public readonly TemplateDelegate TD2;
        public readonly TemplateDelegate TD3;
        public readonly TemplateDelegate[] TDA;
        
        public readonly ChainSegment[] BP0;
        public readonly ChainSegment[][] BPA;

        public readonly object[] A;

        internal Closure(PathInfo pi0, PathInfo pi1, PathInfo pi2, PathInfo pi3, PathInfo[] pia, StrongBox<HelperDescriptorBase> hd0, StrongBox<HelperDescriptorBase> hd1, StrongBox<HelperDescriptorBase> hd2, StrongBox<HelperDescriptorBase> hd3, StrongBox<HelperDescriptorBase>[] hda, StrongBox<BlockHelperDescriptorBase> bhd0, StrongBox<BlockHelperDescriptorBase> bhd1, StrongBox<BlockHelperDescriptorBase> bhd2, StrongBox<BlockHelperDescriptorBase> bhd3, StrongBox<BlockHelperDescriptorBase>[] bhda, TemplateDelegate td0, TemplateDelegate td1, TemplateDelegate td2, TemplateDelegate td3, TemplateDelegate[] tda, ChainSegment[] bp0, ChainSegment[][] bpa, object[] a)
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
            A = a;
        }
    }
}