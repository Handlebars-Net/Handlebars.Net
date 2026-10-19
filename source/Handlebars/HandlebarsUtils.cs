using System;
using HandlebarsDotNet.Compiler;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

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
                case JsonNode node:
                    return IsFalsyJsonNode(node, includeZero);
            }

            if (includeZero) return false;

            // Typed zero checks in likelihood order; avoids IsNumber's isinst cascade
            // followed by Convert.ToBoolean's IConvertible dispatch on the hot path.
            return value switch
            {
                int i => i == 0,
                long l => l == 0L,
                double d => d == 0d,
                float f => f == 0f,
                decimal m => m == 0m,
                byte b8 => b8 == 0,
                sbyte s8 => s8 == 0,
                short s16 => s16 == 0,
                ushort u16 => u16 == 0,
                uint u32 => u32 == 0u,
                ulong u64 => u64 == 0ul,
                _ => false
            };
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

        private static bool IsFalsyJsonNode(JsonNode node, bool includeZero)
        {
            // Parsed values wrap JsonElement; programmatically created values retain their CLR type.
            return node is JsonValue scalar && IsFalsy(scalar.GetValue<object>(), includeZero);
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

            if (value is JsonNode node)
            {
                return node switch
                {
                    JsonObject obj   => obj.Count == 0,
                    JsonArray array  => array.Count == 0,
                    _                => false
                };
            }

            // O(1) emptiness checks that avoid allocating an enumerator for the common
            // collection shapes; the IEnumerable fallback boxes List<T>-style struct enumerators.
            if (value is ICollection collection) return collection.Count == 0;

            return value is IEnumerable enumerable && !enumerable.Any();
        }

    }
}

