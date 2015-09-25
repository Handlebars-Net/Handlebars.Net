using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    /// <summary>
    /// Builds expression trees for often used methods such as ContainsKey and 
    /// </summary>
    internal static class DictionaryExpressionBuilder
    {
        /// <summary>
        /// Builds an expression tree for invoking the ContainsKey method on a IDictionary&lt;,&gt;.
        /// </summary>
        /// <param name="instanceType">Concrete type of the Dictionary.</param>
        /// <remarks>
        /// Implements this method as an Expression Tree:
        /// 
        /// public bool ContainsKey(object dictionary, object key)
        /// {
        ///     var key = (TypeOfKey)key;
        ///     var dictionary = (TypeOfIDictionary)dictionary;
        /// 
        ///     var result = dictionary.ContainsKey(key);
        ///     return result;
        /// }
        /// 
        /// </remarks>
        /// <returns></returns>
        internal static Func<object, object, bool> BuildContainsKeyExpression(Type instanceType)
        {
            var dictType = GetFirstGenericDictionaryTypeInstance(instanceType);
            var keyType = dictType.GetGenericArguments().First();

            ParameterExpression objKeyParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objKey");
            UnaryExpression castKey = System.Linq.Expressions.Expression.Convert(objKeyParam, keyType);
            ParameterExpression keyParam = System.Linq.Expressions.Expression.Parameter(keyType, "key");

            ParameterExpression objDictParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objDict");
            UnaryExpression castDict = System.Linq.Expressions.Expression.Convert(objDictParam, dictType);
            ParameterExpression dictParam = System.Linq.Expressions.Expression.Parameter(dictType, "dict");

            ParameterExpression resultParam = System.Linq.Expressions.Expression.Parameter(typeof(bool), "result");

            LabelTarget returnTarget = System.Linq.Expressions.Expression.Label(typeof(bool));
            var returnLabel = System.Linq.Expressions.Expression.Label(returnTarget,
                System.Linq.Expressions.Expression.Default(typeof(bool)));

            var containsKeyMethod = dictType.GetMethod("ContainsKey");

            var method = System.Linq.Expressions.Expression.Block(
                new[] { keyParam, dictParam, resultParam },
                System.Linq.Expressions.Expression.Assign(keyParam, castKey),
                System.Linq.Expressions.Expression.Assign(dictParam, castDict),
                System.Linq.Expressions.Expression.Assign(resultParam, System.Linq.Expressions.Expression.Call(dictParam, containsKeyMethod, keyParam)),
                System.Linq.Expressions.Expression.Return(returnTarget, resultParam, typeof(bool)),
                returnLabel
            );

            var func = System.Linq.Expressions.Expression.Lambda<Func<object, object, bool>>(method, true, objDictParam, objKeyParam).Compile();
            return func;
        }

        internal static Func<object, object, object> BuildGetItemExpression(Type instanceType)
        {
            var dictType = GetFirstGenericDictionaryTypeInstance(instanceType);
            var genericTypes = dictType.GetGenericArguments();
            var keyType = genericTypes[0];
            var valueType = genericTypes[1];

            ParameterExpression objKeyParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objKey");
            UnaryExpression castKey = System.Linq.Expressions.Expression.Convert(objKeyParam, keyType);
            ParameterExpression keyParam = System.Linq.Expressions.Expression.Parameter(keyType, "key");

            ParameterExpression objDictParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objDict");
            UnaryExpression castDict = System.Linq.Expressions.Expression.Convert(objDictParam, dictType);
            ParameterExpression dictParam = System.Linq.Expressions.Expression.Parameter(dictType, "dict");

            ParameterExpression resultParam = System.Linq.Expressions.Expression.Parameter(valueType, "result");
            LabelTarget returnTarget = System.Linq.Expressions.Expression.Label(valueType);
            var returnLabel = System.Linq.Expressions.Expression.Label(returnTarget,
                System.Linq.Expressions.Expression.Default(valueType));

            var getItemMethod = dictType.GetMethod("get_Item");

            var method = System.Linq.Expressions.Expression.Block(
                new[] { keyParam, dictParam, resultParam },
                System.Linq.Expressions.Expression.Assign(keyParam, castKey),
                System.Linq.Expressions.Expression.Assign(dictParam, castDict),
                System.Linq.Expressions.Expression.Assign(resultParam, System.Linq.Expressions.Expression.Call(dictParam, getItemMethod, keyParam)),
                System.Linq.Expressions.Expression.Return(returnTarget, resultParam, typeof(bool)),
                returnLabel
            );

            var func = System.Linq.Expressions.Expression.Lambda<Func<object, object, object>>(method, true, objDictParam, objKeyParam).Compile();
            return func;
        }

        internal static Type GetFirstGenericDictionaryTypeInstance(Type instanceType)
        {
            var interfaces = instanceType.GetInterfaces();

            var genericInterface = interfaces.FirstOrDefault(i => i.IsGenericType
                                                                  && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            return genericInterface;
        }
    }
}
