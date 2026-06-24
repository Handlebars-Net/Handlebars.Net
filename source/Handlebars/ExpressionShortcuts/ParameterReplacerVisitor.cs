using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.ExpressionShortcuts
{
    internal sealed class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly List<Expression> _replacements;

        public ParameterReplacerVisitor(IEnumerable<Expression?> replacements)
        {
            _replacements = replacements.Where(o => o != null).ToList()!;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var replacement = _replacements.FirstOrDefault(o => o.Type == node.Type);
            if (replacement == null || replacement == node)
            {
                return base.VisitParameter(node);
            }
            
            return Visit(replacement)!;
        }
    }
}