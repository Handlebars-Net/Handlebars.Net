using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Text.RegularExpressions;

namespace HandlebarsDotNet.Compiler
{
    internal class WhitespaceRemover : TokenConverter
    {
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);
        private static readonly Regex MatchLastStartsWithWhitespace = new Regex(@"^[ \t]*(\r?\n|$)", RegexOptions.Compiled, RegexTimeout);
        private static readonly Regex MatchStartsWithWhitespace = new Regex(@"^[ \t]*\r?\n", RegexOptions.Compiled, RegexTimeout);
        private static readonly Regex TrimStartRegex = new Regex(@"^[ \t]*\r?\n?", RegexOptions.Compiled, RegexTimeout);
        private static readonly Regex MatchFirstEndsWithWhitespace = new Regex(@"(^|\r?\n)\s*?$", RegexOptions.Compiled, RegexTimeout);
        private static readonly Regex MatchEndsWithWhitespace = new Regex(@"\r?\n\s*?$", RegexOptions.Compiled, RegexTimeout);
        private static readonly Regex TrimEndRegex = new Regex(@"[ \t]+\z", RegexOptions.Compiled, RegexTimeout);
        // Captures trailing whitespace (spaces/tabs) after the last newline — this is the partial's indentation
        private static readonly Regex ExtractIndentRegex = new Regex(@"(?:^|\r?\n)([ \t]*)$", RegexOptions.Compiled, RegexTimeout);
        
        private static readonly WhitespaceRemover Remover = new WhitespaceRemover();

        public static IEnumerable<object> Remove(IEnumerable<object> sequence)
        {
            return Remover.ConvertTokens(sequence);
        }

        private WhitespaceRemover()
        {
        }

        private static IList<object> ToList(IEnumerable<object> sequence)
        {
            //it's already IList<object> but let's pretend we don't know.
            return sequence as IList<object> ?? sequence.ToArray();
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var list = ToList(sequence);

            ProcessTokens(list);

            return list;
        }

        private static void ProcessTokens(IList<object> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] is StatementExpression statement)
                    ProcessStatement(list, i, statement);
            }
        }

        private static void ProcessStatement(IList<object> list, int index, StatementExpression statement)
        {
            if (statement.TrimBefore) TrimBefore(list, index, true);
            if (statement.TrimAfter) TrimAfter(list, index, true);

            if (!IsStandalone(statement) || !IsNextWhitespace(list, index) || !IsPrevWhitespace(list, index)) return;

            // For standalone partials, extract the preceding indentation and store it on the
            // PartialExpression so it can be applied to every line of the rendered output.
            if (statement.Body is PartialExpression partialExpr)
            {
                var indent = ExtractIndent(list, index);
                if (!string.IsNullOrEmpty(indent))
                {
                    var indentedPartial = HandlebarsExpression.Partial(
                        partialExpr.PartialName,
                        partialExpr.Argument,
                        partialExpr.Fallback,
                        partialExpr.IsBlock,
                        indent);
                    list[index] = HandlebarsExpression.Statement(
                        indentedPartial,
                        statement.IsEscaped,
                        statement.TrimBefore,
                        statement.TrimAfter);
                }
            }

            if (!statement.TrimBefore) TrimBefore(list, index, false);
            if (!statement.TrimAfter) TrimAfter(list, index, false);
        }

        private static string ExtractIndent(IList<object> list, int index)
        {
            if (index < 1) return string.Empty;
            if (!(list[index - 1] is StaticToken prev)) return string.Empty;

            var match = ExtractIndentRegex.Match(prev.Value);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static bool IsNextWhitespace(IList<object> list, int index)
        {
            if (index >= list.Count - 1)
            {
                return true;
            }

            var next = list[index + 1] as StaticToken;

            if (next == null)
            {
                return false;
            }

            var nextIsLast = index == list.Count - 2;

            var regex = nextIsLast ? MatchLastStartsWithWhitespace : MatchStartsWithWhitespace;

            return regex.IsMatch(next.Original);
        }

        private static void TrimAfter(IList<object> list, int index, bool multipleLines)
        {
            if (index >= list.Count - 1)
            {
                return;
            }

            if (!(list[index + 1] is StaticToken next))
            {
                return;
            }

            list[index + 1] = TrimStart(next, multipleLines);
        }

        private static Token TrimStart(StaticToken token, bool multipleLines)
        {
            var value = multipleLines
                ? token.Value.TrimStart()
                : TrimStartRegex.Replace(token.Value, String.Empty);

            return token.GetModifiedToken(value);
        }

        private static bool IsPrevWhitespace(IList<object> list, int index)
        {
            if (index < 1)
            {
                return true;
            }

            if (!(list[index - 1] is StaticToken prev))
            {
                return false;
            }

            var prevIsFirst = index == 1;

            var regex = prevIsFirst ? MatchFirstEndsWithWhitespace : MatchEndsWithWhitespace;

            return regex.IsMatch(prev.Original);
        }

        private static void TrimBefore(IList<object> list, int index, bool multipleLines)
        {
            if (index < 1)
            {
                return;
            }

            if (!(list[index - 1] is StaticToken prev))
            {
                return;
            }

            list[index - 1] = TrimEnd(prev, multipleLines);
        }

        private static Token TrimEnd(StaticToken token, bool multipleLines)
        {
            var value = multipleLines
                ? token.Value.TrimEnd()
                : TrimEndRegex.Replace(token.Value, String.Empty);

            return token.GetModifiedToken(value);
        }

        private static bool IsStandalone(StatementExpression statement)
        {
            return statement.Body is CommentExpression ||
                   statement.Body is PartialExpression ||
                   IsBlockStatement(statement);
        }

        private static bool IsBlockStatement(StatementExpression statement)
        {
            return IsBlockHelperOrInversion(statement.Body as HelperExpression) ||
                   IsSectionOrClosingNode(statement.Body as PathExpression);
        }

        private static bool IsSectionOrClosingNode(PathExpression pathExpression)
        {
            return (pathExpression != null) && pathExpression.Path.IndexOfAny(new[] {'#', '/', '^'}) == 0;
        }

        private static bool IsBlockHelperOrInversion(HelperExpression helperExpression)
        {
            if (helperExpression == null) return false;

            return helperExpression.HelperName.StartsWith("#") || helperExpression.HelperName.StartsWith("^") || (helperExpression.HelperName == "else");
        }
    }
}