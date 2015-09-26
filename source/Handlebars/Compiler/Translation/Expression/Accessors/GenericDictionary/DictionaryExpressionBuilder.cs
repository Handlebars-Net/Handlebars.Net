using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.GenericDictionary
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
        /// <returns>Delegate to invoke the method build using the Expression tree.</returns>
        /// <remarks>
        /// Implements this method using an Expression Tree:
        /// 
        /// public bool ContainsKey(object objDictionary, object objKey)
        /// {
        ///     var key = (TypeOfKey)objKey;
        ///     var dictionary = (TypeOfIDictionary)objDictionary;
        /// 
        ///     var result = dictionary.ContainsKey(key);
        ///     return result;
        /// }
        /// 
        /// </remarks>
        internal static Func<object, object, bool> BuildContainsKeyExpression(Type instanceType)
        {
            var dictType = GetFirstGenericDictionaryTypeInstance(instanceType);
            var keyType = dictType.GetGenericArguments().First();

            ParameterExpression objKeyParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objKey");
            ParameterExpression objDictParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objDict");

            UnaryExpression castKey = System.Linq.Expressions.Expression.Convert(objKeyParam, keyType);
            ParameterExpression keyParam = System.Linq.Expressions.Expression.Parameter(keyType, "key");

            UnaryExpression castDict = System.Linq.Expressions.Expression.Convert(objDictParam, dictType);
            ParameterExpression dictParam = System.Linq.Expressions.Expression.Parameter(dictType, "dict");


            ParameterExpression resultParam = System.Linq.Expressions.Expression.Parameter(typeof(bool), "result");
            LabelTarget returnTarget = System.Linq.Expressions.Expression.Label(typeof(bool));
            var returnLabel = System.Linq.Expressions.Expression.Label(returnTarget,
                System.Linq.Expressions.Expression.Default(typeof(bool)));

            var getItemMethod = dictType.GetMethod("ContainsKey");

            var body = System.Linq.Expressions.Expression.Block(
                new[] { keyParam, dictParam, resultParam },
                System.Linq.Expressions.Expression.Assign(keyParam, castKey),
                System.Linq.Expressions.Expression.Assign(dictParam, castDict),
                System.Linq.Expressions.Expression.Assign(resultParam, System.Linq.Expressions.Expression.Call(dictParam, getItemMethod, keyParam)),
                System.Linq.Expressions.Expression.Return(returnTarget, resultParam, typeof(bool)),
                returnLabel
            );

            var func = System.Linq.Expressions.Expression.Lambda<Func<object, object, bool>>(body, true, objDictParam, objKeyParam).Compile();
            return func;
        }

        /// <summary>
        /// Builds an expression tree for invoking the indexor method on a IDictionary&lt;,&gt;.
        /// </summary>
        /// <param name="instanceType">Concrete type of the dictionary.</param>
        /// <returns>Delegate to invoke the method build using the Expression tree.</returns>
        /// <remarks>
        /// Implements this method using an Expression Tree:
        /// 
        /// public object GetValue(object objDictionary, object objKey) 
        /// {
        ///     var key = (TypeOfKey)objKey;
        ///     var dictionary = (TypeOfIDictionary)objDictionary;
        /// 
        ///     object result = dictionary[key];
        ///     return result;
        /// }
        /// </remarks>
        internal static Func<object, object, object> BuildGetItemExpression(Type instanceType)
        {
            var dictType = GetFirstGenericDictionaryTypeInstance(instanceType);
            var genericTypes = dictType.GetGenericArguments();
            var keyType = genericTypes[0];
            var valueType = genericTypes[1];

            ParameterExpression objKeyParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objKey");
            ParameterExpression objDictParam = System.Linq.Expressions.Expression.Parameter(typeof(object), "objDict");

            UnaryExpression castKey = System.Linq.Expressions.Expression.Convert(objKeyParam, keyType);
            ParameterExpression keyParam = System.Linq.Expressions.Expression.Parameter(keyType, "key");

            UnaryExpression castDict = System.Linq.Expressions.Expression.Convert(objDictParam, dictType);
            ParameterExpression dictParam = System.Linq.Expressions.Expression.Parameter(dictType, "dict");

            ParameterExpression resultParam = System.Linq.Expressions.Expression.Parameter(valueType, "result");
            UnaryExpression boxResult = System.Linq.Expressions.Expression.Convert(resultParam, typeof (object));

            ParameterExpression returnParam = System.Linq.Expressions.Expression.Parameter(typeof (object),
                "returnValue");
            LabelTarget returnTarget = System.Linq.Expressions.Expression.Label(typeof(object));
            var returnLabel = System.Linq.Expressions.Expression.Label(returnTarget,
                System.Linq.Expressions.Expression.Default(typeof(object)));

            var getItemMethod = dictType.GetMethod("get_Item");

            var body = System.Linq.Expressions.Expression.Block(
                new[] { keyParam, dictParam, resultParam, returnParam },
                System.Linq.Expressions.Expression.Assign(keyParam, castKey),
                System.Linq.Expressions.Expression.Assign(dictParam, castDict),
                System.Linq.Expressions.Expression.Assign(resultParam, System.Linq.Expressions.Expression.Call(dictParam, getItemMethod, keyParam)),
                System.Linq.Expressions.Expression.Assign(returnParam, boxResult),
                System.Linq.Expressions.Expression.Return(returnTarget, returnParam, typeof(object)),
                returnLabel
            );

            var func = System.Linq.Expressions.Expression.Lambda<Func<object, object, object>>(body, true, objDictParam, objKeyParam).Compile();
            return func;
        }


        /// <summary>
        /// Obtains the first implemented interface that is a IDictionary&lt;,&gt;.
        /// </summary>
        /// <param name="instanceType">Type of the instance to analyze.</param>
        /// <returns></returns>
        internal static Type GetFirstGenericDictionaryTypeInstance(Type instanceType)
        {
            if (IsGenericDictionaryInterface(instanceType))
            {
                return instanceType;
            }

            var interfaces = instanceType.GetInterfaces();

            var genericInterface = interfaces.FirstOrDefault(IsGenericDictionaryInterface);
            return genericInterface;
        }

        private static bool IsGenericDictionaryInterface(Type i)
        {
            var isGenDict = i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>);
            return isGenDict;
        }
    }
}
