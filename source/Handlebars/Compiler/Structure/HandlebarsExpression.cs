using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Handlebars.Compiler
{
    internal enum HandlebarsExpressionType
    {
        StaticExpression = 6000,
        StatementExpression = 6001,
        BlockExpression = 6002,
        HelperExpression = 6003,
        PathExpression = 6004,
        ContextAccessorExpression = 6005,
        IteratorExpression = 6006,
        DeferredSection = 6007,
        PartialExpression = 6008,
		BoolishExpression = 6009
    }

    internal abstract class HandlebarsExpression : Expression
    {
        public static HelperExpression Helper(string helperName, IEnumerable<Expression> arguments)
        {
            return new HelperExpression(helperName, arguments);
        }

        public static HelperExpression Helper(string helperName)
        {
            return new HelperExpression(helperName);
        }

        public static BlockHelperExpression BlockHelper(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body)
        {
            return new BlockHelperExpression(helperName, arguments, body);
        }

        public static PathExpression Path(string path)
        {
            return new PathExpression(path);
        }

        public static StaticExpression Static(string value)
        {
            return new StaticExpression(value);
        }

        public static ContextAccessorExpression ContextAccessor()
        {
            return new ContextAccessorExpression();
        }

		public static StatementExpression Statement(Expression body, bool isEscaped)
        {
			return new StatementExpression(body, isEscaped);
        }

        public static IteratorExpression Iterator(
            Expression sequence,
            Expression template)
        {
            return new IteratorExpression(sequence, template);
        }

        public static IteratorExpression Iterator(
            Expression sequence,
            Expression template,
            Expression ifEmpty)
        {
            return new IteratorExpression(sequence, template, ifEmpty);
        }

        public static DeferredSectionExpression DeferredSection(
            PathExpression path,
            IEnumerable<Expression> body,
            SectionEvaluationMode evalMode)
        {
            return new DeferredSectionExpression(path, body, evalMode);
        }

        public static PartialExpression Partial(string partialName)
        {
            return new PartialExpression(partialName);
        }

		public static BoolishExpression Boolish(Expression condition)
		{
			return new BoolishExpression(condition);
		}
    }
}

