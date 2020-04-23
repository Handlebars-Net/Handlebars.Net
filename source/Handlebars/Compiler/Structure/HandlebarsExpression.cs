using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal enum HandlebarsExpressionType
    {
        StaticExpression = 6000,
        StatementExpression = 6001,
        BlockExpression = 6002,
        HelperExpression = 6003,
        PathExpression = 6004,
        IteratorExpression = 6005,
        PartialExpression = 6007,
        BoolishExpression = 6008,
        SubExpression = 6009,
        HashParameterAssignmentExpression = 6010,
        HashParametersExpression = 6011,
        CommentExpression = 6012,
        BlockParamsExpression = 6013
    }

    internal abstract class HandlebarsExpression : Expression
    {
        public override Type Type => GetType();

        public override bool CanReduce { get; } = false;

        public static HelperExpression Helper(string helperName, bool isBlock, IEnumerable<Expression> arguments, bool isRaw = false)
        {
            return new HelperExpression(helperName, isBlock, arguments, isRaw);
        }

        public static HelperExpression Helper(string helperName, bool isBlock, bool isRaw = false, IReaderContext context = null)
        {
            return new HelperExpression(helperName, isBlock, isRaw, context);
        }

        public static BlockHelperExpression BlockHelper(
            string helperName,
            IEnumerable<Expression> arguments,
            BlockParamsExpression blockParams,
            Expression body,
            Expression inversion,
            bool isRaw = false)
        {
            return new BlockHelperExpression(helperName, arguments, blockParams, body, inversion, isRaw);
        }

        public static PathExpression Path(string path)
        {
            return new PathExpression(path);
        }
        
        public static BlockParamsExpression BlockParams(string action, string blockParams)
        {
            return new BlockParamsExpression(action, blockParams);
        }

        public static StaticExpression Static(string value)
        {
            return new StaticExpression(value);
        }

        public static StatementExpression Statement(Expression body, bool isEscaped, bool trimBefore, bool trimAfter)
        {
            return new StatementExpression(body, isEscaped, trimBefore, trimAfter);
        }

        public static IteratorExpression Iterator(
            Expression sequence,
            BlockParamsExpression blockParams,
            Expression template)
        {
            return new IteratorExpression(sequence, blockParams, template, Empty());
        }

        public static IteratorExpression Iterator(
            Expression sequence,
            BlockParamsExpression blockParams,
            Expression template,
            Expression ifEmpty)
        {
            return new IteratorExpression(sequence, blockParams, template, ifEmpty);
        }

        public static PartialExpression Partial(Expression partialName)
        {
            return Partial(partialName, null);
        }

        public static PartialExpression Partial(Expression partialName, Expression argument)
        {
            return new PartialExpression(partialName, argument, null);
        }

        public static PartialExpression Partial(Expression partialName, Expression argument, Expression fallback)
        {
            return new PartialExpression(partialName, argument, fallback);
        }

        public static BoolishExpression Boolish(Expression condition)
        {
            return new BoolishExpression(condition);
        }

        public static SubExpressionExpression SubExpression(Expression expression)
        {
            return new SubExpressionExpression(expression);
        }

        public static HashParameterAssignmentExpression HashParameterAssignmentExpression(string name)
        {
            return new HashParameterAssignmentExpression(name);
        }

        public static HashParametersExpression HashParametersExpression(Dictionary<string, Expression> parameters)
        {
            return new HashParametersExpression(parameters);
        }

        public static CommentExpression Comment(string value)
        {
            return new CommentExpression(value);
        }
    }
}

