using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperConverter : TokenConverter
    {
        private static readonly HashSet<string> BuiltInHelpers = new HashSet<string>
        {
            "else", "each"
        };

        public static IEnumerable<object> Convert(
            IEnumerable<object> sequence,
            ICompiledHandlebarsConfiguration configuration)
        {
            return new HelperConverter(configuration).ConvertTokens(sequence).ToList();
        }

        private readonly ICompiledHandlebarsConfiguration _configuration;

        private HelperConverter(ICompiledHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (!(item is StartExpressionToken token))
                {
                    yield return item;
                    continue;
                }

                var isRaw = token.IsRaw;
                yield return token;
                item = GetNext(enumerator);
                switch (item)
                {
                    case Expression _:
                        yield return item;
                        continue;
                    case WordExpressionToken word when IsRegisteredHelperName(word.Value):
                        yield return HandlebarsExpression.Helper(word.Value, false, isRaw, word.Context);
                        break;
                    case WordExpressionToken word when IsRegisteredBlockHelperName(word.Value, isRaw):
                    {
                        yield return HandlebarsExpression.Helper(word.Value, true, isRaw, word.Context);
                        break;
                    }
                    case WordExpressionToken word when IsUnregisteredBlockHelperName(word.Value, isRaw, sequence):
                    {
                        var expression = HandlebarsExpression.Helper(word.Value, true, isRaw, word.Context);
                        expression.IsBlock = true;
                        yield return expression;
                        break;
                    }
                    default:
                        yield return item;
                        break;
                }
            }
        }

        private bool IsRegisteredHelperName(string name) => IsRegisteredHelperName(name, _configuration);

        internal static bool IsRegisteredHelperName(string name, ICompiledHandlebarsConfiguration configuration)
        {
            var pathInfo = PathInfo.Parse(name);
#pragma warning disable CS0618
            if (!pathInfo.IsValidHelperLiteral && !configuration.Compatibility.RelaxedHelperNaming) return false;
#pragma warning restore CS0618
            if (pathInfo.IsBlockHelper || pathInfo.IsInversion || pathInfo.IsBlockClose || pathInfo.IsThis) return false;
            name = pathInfo.TrimmedPath!;

            return configuration.Helpers.ContainsKey(pathInfo) || configuration.Decorators.ContainsKey(pathInfo) || BuiltInHelpers.Contains(name);
        }

        private bool IsRegisteredBlockHelperName(string name, bool isRaw)
        {
            var pathInfo = PathInfo.Parse(name);
#pragma warning disable CS0618
            if (!pathInfo.IsValidHelperLiteral && !_configuration.Compatibility.RelaxedHelperNaming) return false;
#pragma warning restore CS0618
            if (!isRaw && !(pathInfo.IsBlockHelper || pathInfo.IsInversion)) return false;
            if (pathInfo.IsBlockClose) return false;
            if (pathInfo.IsThis) return false;

            name = pathInfo.TrimmedPath!;
            
            return _configuration.BlockHelpers.ContainsKey(pathInfo) || _configuration.BlockDecorators.ContainsKey(pathInfo) || BuiltInHelpers.Contains(name);
        }
        
        private bool IsUnregisteredBlockHelperName(string name, bool isRaw, IEnumerable<object> sequence)
        {
            var pathInfo = PathInfo.Parse(name);
#pragma warning disable CS0618
            if (!pathInfo.IsValidHelperLiteral && !_configuration.Compatibility.RelaxedHelperNaming && !IsCurrentContextBlock(name)) return false;
#pragma warning restore CS0618

            if (!isRaw && !(pathInfo.IsBlockHelper || pathInfo.IsInversion)) return false;
            if (pathInfo.IsBlockClose) return false;
            name = name.Substring(1);
            if (name.StartsWith("*"))
            {
                name = name.Substring(1);
            }

            var expectedBlockName = $"/{name}";
            return sequence.OfType<WordExpressionToken>().Any(o =>
                string.Equals(o.Value, expectedBlockName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// "{{#.}}" / "{{^.}}" open a section over the current context, the same as "{{#this}}" / "{{^this}}".
        /// </summary>
        private static bool IsCurrentContextBlock(string name)
        {
            return name.Length == 2 && (name[0] == '#' || name[0] == '^') && name[1] == '.';
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

