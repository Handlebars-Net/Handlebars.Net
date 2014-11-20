using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
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
            if(exp == null)
            {
                return null;
            }
            switch((HandlebarsExpressionType)exp.NodeType)
            {
                case HandlebarsExpressionType.StatementExpression:
                    return VisitStatementExpressionCore((StatementExpression)exp);
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
                case HandlebarsExpressionType.DeferredSection:
                    return VisitDeferredSectionExpression((DeferredSectionExpression)exp);
                case HandlebarsExpressionType.PartialExpression:
                    return VisitPartialExpression((PartialExpression)exp);
				case HandlebarsExpressionType.BoolishExpression:
					return VisitBoolishExpression((BoolishExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

		private Expression VisitStatementExpressionCore(StatementExpression sex)
		{
			return Expression.Block(
				Expression.Assign(
					Expression.Property(CompilationContext.BindingContext, "OutputMode"),
					Expression.Constant(sex.IsEscaped ? OutputMode.Encoded : OutputMode.Unencoded)),
				VisitStatementExpression(sex));
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

        protected virtual Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            return dsex;
        }

        protected virtual Expression VisitPartialExpression(PartialExpression pex)
        {
            return pex;
        }

		protected virtual Expression VisitBoolishExpression(BoolishExpression bex)
		{
			return bex;
		}
    }
}

