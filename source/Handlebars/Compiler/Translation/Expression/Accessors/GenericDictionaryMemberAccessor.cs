using System;
using System.Globalization;
using System.Linq;

namespace HandlebarsDotNet.Compiler.Translation.Expression.Accessors
{
    internal class GenericDictionaryMemberAccessor : IMemberAccessor
    {
        //private static IDictionary<Type, Func<object, object, bool>> ContainsKeysFunc; 

        static GenericDictionaryMemberAccessor()
        {
            //typeof(IDictionary<string, string>).GetMethod("")
        }

        public bool RequiresResolvedMemberName { get { return true; } }

        public bool CanHandle(object instance)
        {
            var instanceType = instance.GetType();
            var canHandle = ImplementsGenericDictionaryInterface(instanceType);
            
            return canHandle;
        }

        private bool ImplementsGenericDictionaryInterface(Type instanceType)
        {
            var dictInterfaceType = DictionaryExpressionBuilder.GetFirstGenericDictionaryTypeInstance(instanceType);

            return dictInterfaceType != null;
        }

        //TODO: Here!
        public object AccessMember(object instance, string memberName)
        {
            var instanceType = instance.GetType();

            var key = GetKey(instanceType, memberName);
            if (key == null) { return new UndefinedBindingResult(); }

            throw new NotImplementedException();
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
