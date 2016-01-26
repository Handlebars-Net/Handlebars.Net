using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.GenericDictionary;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal sealed class GenericDictionaryMemberAccessor : IMemberAccessor
    {
        //TODO: Maybe use a sliding cache or something. I'm afraid for long running processes with lots of different types.
        private static readonly IDictionary<Type, GenericDictionaryProxy> Proxies = new Dictionary<Type, GenericDictionaryProxy>();

        public bool RequiresResolvedMemberName { get { return true; } }

        private static GenericDictionaryProxy GetDictionaryProxy(Type instanceType)
        {
            var dictType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);

            GenericDictionaryProxy proxy;
            if (!Proxies.TryGetValue(dictType, out proxy))
            {
                proxy = new GenericDictionaryProxy(dictType);
                Proxies.AddOrUpdate(dictType, proxy);
            }

            return proxy;
        }

        public bool CanHandle(object instance, string memberName)
        {
            if (instance == null) { return false; }

            var instanceType = instance.GetType();
            var dictType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);

            if (dictType != null)
            {
                var genericTypes = dictType.GetGenericArguments();
                var keyType = genericTypes[0];

                //Optimization. When using string key, we will use the IDictionary (non-generic) implementation.
                var shouldOptimize = keyType == typeof (string) && typeof (IDictionary).IsAssignableFrom(instanceType);

                return !shouldOptimize;
            }
            
            return false;
        }

        private bool ImplementsGenericDictionaryInterface(Type instanceType)
        {
            var dictInterfaceType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);

            return dictInterfaceType != null;
        }

        public object AccessMember(object instance, string memberName)
        {
            var instanceType = instance.GetType();

            var key = GetKey(instanceType, memberName);
            if (key == null) { return new UndefinedBindingResult(); }

            var proxy = GetDictionaryProxy(instanceType);

            if (proxy.ContainsKey(instance, key))
            {
                var value = proxy.GetValue(instance, key);
                return value;
            }

            return new UndefinedBindingResult();
        }

        private object GetKey(Type instanceType, string memberName)
        {
            memberName = CleanMemberName(memberName);

            var dictType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);
            var keyType = dictType.GetGenericArguments().First();

            if(keyType == typeof(string)) { return memberName; }

            try
            {
                var key = Convert.ChangeType(memberName, keyType, CultureInfo.CurrentCulture);
                return key;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private static string CleanMemberName(string memberName)
        {
            return memberName.Trim('[', ']');
        }
    }
}
