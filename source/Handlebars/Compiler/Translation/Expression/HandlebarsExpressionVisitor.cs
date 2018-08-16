using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class HandlebarsExpressionVisitor : ExpressionVisitor
    {
        private readonly CompilationContext _compilationContext;

        protected HandlebarsExpressionVisitor(CompilationContext compilationContext)
        {
            _compilationContext = compilationContext;
        }

        protected virtual CompilationContext CompilationContext
        {
            get { return _compilationContext; }
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch ((HandlebarsExpressionType)exp.NodeType)
            {
                case HandlebarsExpressionType.StatementExpression:
                    return VisitStatementExpression((StatementExpression)exp);
                case HandlebarsExpressionType.StaticExpression:
                    return VisitStaticExpression((StaticExpression)exp);
                case HandlebarsExpressionType.HelperExpression:
                    return VisitHelperExpression((HelperExpression)exp);
                case HandlebarsExpressionType.BlockExpression:
                    return VisitBlockHelperExpression((BlockHelperExpression)exp);
                case HandlebarsExpressionType.HashParametersExpression:
                    return VisitHashParametersExpression((HashParametersExpression)exp);
                case HandlebarsExpressionType.PathExpression:
                    return VisitPathExpression((PathExpression)exp);
                case HandlebarsExpressionType.IteratorExpression:
                    return VisitIteratorExpression((IteratorExpression)exp);
                case HandlebarsExpressionType.DeferredSection:
                    return VisitDeferredSectionExpression((DeferredSectionExpression)exp);
                case HandlebarsExpressionType.PartialExpression:
                    return VisitPartialExpression((PartialExpression)exp);
                case HandlebarsExpressionType.BoolishExpression:
                    return VisitBoolishExpression((BoolishExpression)exp);
                case HandlebarsExpressionType.SubExpression:
                    return VisitSubExpression((SubExpressionExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitStatementExpression(StatementExpression sex)
        {
            Expression body = Visit(sex.Body);
            if (body != sex.Body)
            {
                return HandlebarsExpression.Statement(body, sex.IsEscaped, sex.TrimBefore, sex.TrimAfter);
            }
            return sex;
        }

        protected virtual Expression VisitPathExpression(PathExpression pex)
        {
            return pex;
        }

        protected virtual Expression VisitHelperExpression(HelperExpression hex)
        {
            var arguments = VisitExpressionList(hex.Arguments);
            if (arguments != hex.Arguments)
            {
                return HandlebarsExpression.Helper(hex.HelperName, arguments);
            }
            return hex;
        }

        protected virtual Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var arguments = VisitExpressionList(bhex.Arguments);
            Expression body = Visit(bhex.Body);
            Expression inversion = Visit(bhex.Inversion);

            if (arguments != bhex.Arguments
                || body != bhex.Body
                || inversion != bhex.Inversion)
            {
                return HandlebarsExpression.BlockHelper(bhex.HelperName, arguments, body, inversion);
            }
            return bhex;
        }

        protected virtual Expression VisitStaticExpression(StaticExpression stex)
        {
            return stex;
        }

        protected virtual Expression VisitIteratorExpression(IteratorExpression iex)
        {
            Expression sequence = Visit(iex.Sequence);
            Expression template = Visit(iex.Template);
            Expression ifEmpty = Visit(iex.IfEmpty);
            if (sequence != iex.Sequence
                || template != iex.Template
                || ifEmpty != iex.IfEmpty)
            {
                return HandlebarsExpression.Iterator(sequence, template, ifEmpty);
            }
            return iex;
        }

        protected virtual Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            PathExpression path = (PathExpression)Visit(dsex.Path);
            BlockExpression body = (BlockExpression)VisitBlock(dsex.Body);
            BlockExpression inversion = (BlockExpression)VisitBlock(dsex.Inversion);
            if (path != dsex.Path
                || body != dsex.Body
                || inversion != dsex.Inversion)
            {
                return HandlebarsExpression.DeferredSection(path, body, inversion);
            }
            return dsex;
        }

        protected virtual Expression VisitPartialExpression(PartialExpression pex)
        {
            Expression partialName = Visit(pex.PartialName);
            Expression argument = Visit(pex.Argument);
            Expression fallback = Visit(pex.Fallback);
            if (partialName != pex.PartialName
                || argument != pex.Argument
                || fallback != pex.Fallback)
            {
                return HandlebarsExpression.Partial(partialName, argument, fallback);
            }
            return pex;
        }

        protected virtual Expression VisitBoolishExpression(BoolishExpression bex)
        {
            Expression condition = Visit(bex.Condition);
            if (condition != bex.Condition)
            {
                return HandlebarsExpression.Boolish(condition);
            }
            return bex;
        }

        protected virtual Expression VisitSubExpression(SubExpressionExpression subex)
        {
            Expression expression = Visit(subex.Expression);
            if (expression != subex.Expression)
            {
                return HandlebarsExpression.SubExpression(expression);
            }
            return subex;
        }

        protected virtual Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            var parameters = new HashParameterDictionary();
            bool parametersChanged = false;
            foreach (string key in hpex.Parameters.Keys)
            {
                Expression value = Visit((Expression)hpex.Parameters[key]);
                parameters.Add(key, value);
                if (value != hpex.Parameters[key])
                {
                    parametersChanged = true;
                }
            }
            if (parametersChanged)
            {
                return HandlebarsExpression.HashParametersExpression(parameters);
            }
            return hpex;
        }

        IEnumerable<Expression> VisitExpressionList(IEnumerable<Expression> original)
        {
            if (original == null)
            {
                return original;
            }

            var originalAsList = original as IReadOnlyList<Expression> ?? original.ToArray();
            List<Expression> list = null;
            for (int i = 0, n = originalAsList.Count; i < n; i++)
            {
                Expression p = Visit(originalAsList[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != originalAsList[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(originalAsList[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
                return list.ToArray();
            return original;
        }
    }
}

