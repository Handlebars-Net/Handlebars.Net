using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        public PartialExpression(Expression partialName, Expression? argument, Expression? fallback, bool isBlock = false, string? indent = null)
        {
            PartialName = partialName;
            Argument = argument;
            Fallback = fallback;
            IsBlock = isBlock;
            Indent = indent;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.PartialExpression;

        public Expression PartialName { get; }

        public Expression? Argument { get; }

        public Expression? Fallback { get; }

        public bool IsBlock { get; }

        /// <summary>
        /// The whitespace that preceded the partial tag on its line.
        /// When non-null/non-empty, this indentation is prepended to every line of the rendered partial output,
        /// matching Handlebars.js standalone partial indentation behavior.
        /// </summary>
        public string? Indent { get; }
    }
}

