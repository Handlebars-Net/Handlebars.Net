using System;
using HandlebarsDotNet.Compiler;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

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
                case JsonElement element:
                    return IsFalsyJsonElement(element, includeZero);
            }

            if (IsNumber(value) && !includeZero)
            {
                return !Convert.ToBoolean(value);
            }
            return false;
        }

        private static bool IsFalsyJsonElement(JsonElement element, bool includeZero)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.False:
                    return true;
                case JsonValueKind.True:
                    return false;
                case JsonValueKind.String:
                    return element.GetString() == string.Empty;
                case JsonValueKind.Number:
                    return !includeZero && element.GetDouble() == 0;
                default:
                    return false;
            }
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

            if (value is JsonElement element)
            {
                return (element.ValueKind == JsonValueKind.Object && !element.EnumerateObject().Any())
                    || (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() == 0);
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

