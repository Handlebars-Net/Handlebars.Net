using System;
using HandlebarsDotNet.Compiler;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet
{
    public static class HandlebarsUtils
    {
        /// <summary>
        /// Implementation of JS's `==`
        /// </summary>
        /// <param name="value"></param>
        /// <param name="includeZero">Determined whether the numerical <c>0</c> is considered to be truthy. Default <c>false</c></param>
        /// <returns></returns>
        public static bool IsTruthy([NotNullWhen(true)] object? value, bool includeZero = false)
        {
            return !IsFalsy(value, includeZero);
        }

        /// <summary>
        /// Implementation of JS's `!=`
        /// </summary>
        /// <param name="value">The value whose falsy-ness is to be determined</param>
        /// <param name="includeZero">Determined whether the numerical <c>0</c> is considered to be truthy</param>
        /// <returns></returns>
        public static bool IsFalsy([NotNullWhen(false)] object? value, bool includeZero)
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
                case SafeString safe:
                    return safe.Value == string.Empty;
            }

            if (IsNumber(value) && !includeZero)
            {
                return !Convert.ToBoolean(value);
            }
            return false;
        }
        
        public static bool IsTruthyOrNonEmpty([NotNullWhen(true)] object? value, bool includeZero = false)
        {
            return !IsFalsyOrEmpty(value, includeZero);
        }
        
        public static bool IsFalsyOrEmpty([NotNullWhen(false)] object? value, bool includeZero = false)
        {
            if(IsFalsy(value, includeZero))
            {
                return true;
            }

            return value is IEnumerable enumerable && !enumerable.Any();
        }

        private static bool IsNumber([NotNullWhen(true)] object? value)
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

