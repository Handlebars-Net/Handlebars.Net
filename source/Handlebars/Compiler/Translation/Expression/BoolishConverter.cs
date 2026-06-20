using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        private readonly CompilationContext _compilationContext;

        public BoolishConverter(CompilationContext compilationContext)
        {
            _compilationContext = compilationContext;
        }

        private const string IncludeZero = "includeZero";

        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            var condition = Visit(bex.Condition);
            condition = FunctionBuilder.Reduce(condition, _compilationContext, out _);
            var hashParameters = (HashParametersExpression)VisitHashParametersExpression(bex.HashParameters);
            // Assert that if there is a hashParameter, it is "includeZero" and has a boolean value
            if (hashParameters.Parameters.Count > 1 || (hashParameters.Parameters.Count == 1 && !hashParameters.Parameters.ContainsKey(IncludeZero)))
            {
                throw new HandlebarsCompilerException($"Boolish expression can only have up to one hash parameter, '{IncludeZero}'");
            }

            var @object = Arg<object>(condition);
            var includeZero = Arg<bool>(hashParameters.Parameters.Count == 1 ? hashParameters.Parameters[IncludeZero] : Expression.Constant(false));
            return Call(() => HandlebarsUtils.IsTruthyOrNonEmpty(@object, includeZero));
        }
    }
}

