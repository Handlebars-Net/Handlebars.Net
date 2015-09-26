using System;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors.GenericDictionary
{
    internal sealed class GenericDictionaryProxy
    {
        private readonly Func<object, object, bool> _containsKeyFunc;
        private readonly Func<object, object, object> _getItemFunc;

        public Type DictionaryType { get; private set; }

        public GenericDictionaryProxy(Type instanceType)
        {
            DictionaryType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);

            _containsKeyFunc = DictionaryExpressionBuilder.BuildContainsKeyExpression(DictionaryType);
            _getItemFunc = DictionaryExpressionBuilder.BuildGetItemExpression(DictionaryType);
        }

        public bool ContainsKey(object dictionaryInstance, object key)
        {
            var value = _containsKeyFunc.Invoke(dictionaryInstance, key);
            return value;
        }

        public object GetValue(object dictionaryInstance, object key)
        {
            var value = _getItemFunc.Invoke(dictionaryInstance, key);
            return value;
        }
    }
}
