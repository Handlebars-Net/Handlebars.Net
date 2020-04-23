using System;
using HandlebarsDotNet.Compiler;
using System.Collections;
using System.Linq;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public static class HandlebarsUtils
    {
        /// <summary>
        /// Implementation of JS's `==`
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTruthy(object value)
        {
            return !IsFalsy(value);
        }

        /// <summary>
        /// Implementation of JS's `!=`
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalsy(object value)
        {
            switch (value)
            {
                case UndefinedBindingResult _:
                case null:
                    return true;
                case bool b:
                    return !b;
                case string s:
                    return s == string.Empty;
            }

            if (IsNumber(value))
            {
                return !Convert.ToBoolean(value);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTruthyOrNonEmpty(object value)
        {
            return !IsFalsyOrEmpty(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalsyOrEmpty(object value)
        {
            if(IsFalsy(value))
            {
                return true;
            }

            return value is IEnumerable enumerable && enumerable.OfType<object>().Any() == false;
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

