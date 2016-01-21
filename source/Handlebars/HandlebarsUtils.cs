using System;
using HandlebarsDotNet.Compiler;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace HandlebarsDotNet
{
    public static class HandlebarsUtils
    {
        public static bool IsTruthy(object value)
        {
            return !IsFalsy(value);
        }

        public static bool IsFalsy(object value)
        {
            if (value is UndefinedBindingResult)
            {
                return true;
            }

            if (value == null)
            {
                return true;
            }

            if (value is bool)
            {
                return !(bool)value;
            }

            if (value is string) {
                if ((string)value == "")
                {
                    return true;
                }

                return false;
            }

            if (IsNumber(value))
            {
                return !Convert.ToBoolean(value);
            }

            if (value.GetType().FullName == "Newtonsoft.Json.Linq.JValue") {
                if (IsStringType(value))
                    return IsFalsy(value.ToString());

                try {
                    return IsFalsy(Convert.ToBoolean(value));
                } catch (Exception) {}
            }

            return false;
        }

        private static PropertyInfo _typeProperty;
        private static bool IsStringType(object value) {
            if (_typeProperty == null)
                _typeProperty = value.GetType().GetProperty("Type");

            var propertyValue = _typeProperty.GetValue(value, new object[] {}).ToString();
            if (propertyValue == "String" || propertyValue == "Null")
                return true;

            return false;
        }

        public static bool IsTruthyOrNonEmpty(object value)
        {
            return !IsFalsyOrEmpty(value);
        }

        public static bool IsFalsyOrEmpty(object value)
        {
            if(IsFalsy(value))
            {
                return true;
            }
            else if (value is IEnumerable && value.GetType().FullName != "Newtonsoft.Json.Linq.JValue" && ((IEnumerable)value).OfType<object>().Any() == false)
            {
                return true;
            }
            return false;
        }

        private static bool IsNumber(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }
    }
}

