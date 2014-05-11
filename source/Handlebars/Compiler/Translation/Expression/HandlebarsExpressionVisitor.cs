using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal abstract class HandlebarsExpressionVisitor : ExpressionVisitor
    {
        public override Expression Visit(Expression exp)
        {
            if(exp == null)
            {
                return null;
            }
            switch((HandlebarsExpressionType)exp.NodeType)
            {
                case HandlebarsExpressionType.StatementExpression:
                    return VisitStatementExpression((StatementExpression)exp);
                case HandlebarsExpressionType.StaticExpression:
                    return VisitStaticExpression((StaticExpression)exp);
                case HandlebarsExpressionType.HelperExpression:
                    return VisitHelperExpression((HelperExpression)exp);
                case HandlebarsExpressionType.BlockExpression:
                    return VisitBlockHelperExpression((BlockHelperExpression)exp);
                case HandlebarsExpressionType.PathExpression:
                    return VisitPathExpression((PathExpression)exp);
                case HandlebarsExpressionType.ContextAccessorExpression:
                    return VisitContextAccessorExpression((ContextAccessorExpression)exp);
                case HandlebarsExpressionType.IteratorExpression:
                    return VisitIteratorExpression((IteratorExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitContextAccessorExpression(ContextAccessorExpression caex)
        {
            return caex;
        }

        protected virtual Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex;
        }

        protected virtual Expression VisitPathExpression(PathExpression pex)
        {
            return pex;
        }

        protected virtual Expression VisitHelperExpression(HelperExpression hex)
        {
            return hex;
        }

        protected virtual Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return bhex;
        }

        protected virtual Expression VisitStaticExpression(StaticExpression stex)
        {
            return stex;
        }

        protected virtual Expression VisitIteratorExpression(IteratorExpression iex)
        {
            return iex;
        }
    }
}

