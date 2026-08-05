using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HandlebarsExpressionVisitor : ExpressionVisitor
    {
        [return: NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            if (node == null)
            {
                return null;
            }
            switch ((HandlebarsExpressionType)node.NodeType)
            {
                case HandlebarsExpressionType.StatementExpression:
                    return VisitStatementExpression((StatementExpression)node);
                case HandlebarsExpressionType.StaticExpression:
                    return VisitStaticExpression((StaticExpression)node);
                case HandlebarsExpressionType.HelperExpression:
                    return VisitHelperExpression((HelperExpression)node);
                case HandlebarsExpressionType.BlockExpression:
                    return VisitBlockHelperExpression((BlockHelperExpression)node);
                case HandlebarsExpressionType.HashParameterAssignmentExpression:
                    return node;
                case HandlebarsExpressionType.HashParametersExpression:
                    return VisitHashParametersExpression((HashParametersExpression)node);
                case HandlebarsExpressionType.PathExpression:
                    return VisitPathExpression((PathExpression)node);
                case HandlebarsExpressionType.IteratorExpression:
                    return VisitIteratorExpression((IteratorExpression)node);
                case HandlebarsExpressionType.PartialExpression:
                    return VisitPartialExpression((PartialExpression)node);
                case HandlebarsExpressionType.BoolishExpression:
                    return VisitBoolishExpression((BoolishExpression)node);
                case HandlebarsExpressionType.SubExpression:
                    return VisitSubExpression((SubExpressionExpression)node);
                default:
                    return base.Visit(node);
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
            if (!Equals(arguments, hex.Arguments))
            {
                return HandlebarsExpression.Helper(hex.HelperName, hex.IsBlock, arguments, hex.IsRaw);
            }
            return hex;
        }

        protected virtual Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            var arguments = VisitExpressionList(bhex.Arguments);
            // Don't visit Body/Inversion - they will be compiled separately

            if (arguments != bhex.Arguments)
            {
                return HandlebarsExpression.BlockHelper(bhex.HelperName, arguments, bhex.BlockParams, bhex.Body, bhex.Inversion, bhex.IsRaw);
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
            // Don't visit Template/IfEmpty - they will be compiled separately

            if (sequence != iex.Sequence)
            {
                return HandlebarsExpression.Iterator(iex.HelperName, sequence, iex.BlockParams, iex.Template, iex.IfEmpty);
            }
            return iex;
        }

        protected virtual Expression VisitPartialExpression(PartialExpression pex)
        {
            Expression partialName = Visit(pex.PartialName);
            Expression? argument = Visit(pex.Argument);
            // Don't visit Fallback - it will be compiled separately

            if (partialName != pex.PartialName
                || argument != pex.Argument)
            {
                return HandlebarsExpression.Partial(partialName, argument, pex.Fallback, pex.IsBlock, pex.Indent);
            }
            return pex;
        }

        protected virtual Expression VisitBoolishExpression(BoolishExpression bex)
        {
            Expression condition = Visit(bex.Condition);
            HashParametersExpression hashParameters = (HashParametersExpression)VisitHashParametersExpression(bex.HashParameters);
            if (condition != bex.Condition)
            {
                return HandlebarsExpression.Boolish(condition, hashParameters);
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
            var parameters = new Dictionary<string, Expression>();
            bool parametersChanged = false;
            foreach (string key in hpex.Parameters.Keys)
            {
                Expression value = Visit(hpex.Parameters[key]);
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

        [return: NotNullIfNotNull(nameof(original))]
        private IEnumerable<Expression>? VisitExpressionList(IEnumerable<Expression>? original)
        {
            if (original == null) return Array.Empty<Expression>();

            var originalAsList = original as IReadOnlyList<Expression> ?? original.ToArray();
            List<Expression>? list = null;
            for (int index = 0; index < originalAsList.Count; index++)
            {
                var p = Visit(originalAsList[index]);
                if (list != null)
                {
                    list.Add(p);
                    continue;
                }

                if (p == originalAsList[index]) continue;
                
                list = new List<Expression>(originalAsList.Count);
                for (var j = 0; j < index; j++)
                {
                    list.Add(originalAsList[j]);
                }
                list.Add(p);
            }
            
            return list?.ToArray() ?? originalAsList;
        }
    }
}

