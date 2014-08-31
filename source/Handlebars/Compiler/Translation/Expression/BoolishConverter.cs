using System;
using System.Linq;
using Handlebars.Compiler;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
	internal class BoolishConverter : HandlebarsExpressionVisitor
	{
		public static Expression Convert(Expression expr, CompilationContext context)
		{
			return new BoolishConverter(context).Visit(expr);
		}

		private readonly CompilationContext _context;

		private BoolishConverter(CompilationContext context)
		{
			_context = context;
		}

		protected override Expression VisitBoolishExpression(BoolishExpression bex)
		{
			return Expression.Not(
				Expression.Call(
					new Func<object, bool>(IsFalsy).Method,
					Visit(bex.Condition)));
		}

		protected override Expression VisitBlock(BlockExpression node)
		{
			return Expression.Block(
				node.Variables,
				node.Expressions.Select(expr => Visit(expr)));
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			return Expression.MakeUnary(
				node.NodeType,
				Visit(node.Operand),
				node.Type);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			return Expression.Call(
				Visit(node.Object),
				node.Method,
				node.Arguments.Select(n => Visit(n)));
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			return Expression.Condition(
				Visit(node.Test),
				Visit(node.IfTrue),
				Visit(node.IfFalse));
		}

		private static bool IsFalsy(object value)
		{
			if(value is UndefinedBindingResult)
			{
				return true;
			}
			if(value == null)
			{
				return true;
			}
			else if(value is bool)
			{
				return !(bool)value;
			}
			else if(value is string)
			{
				if((string)value == "")
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if(IsNumber(value))
			{
				return !System.Convert.ToBoolean(value);
			}
			return false;
		}

		private static bool IsNumber(object value)
		{
			return value is sbyte
				|| value is byte
				|| value is short
				|| value is ushort
				|| value is int
				|| value is uint
				|| value is long
				|| value is ulong
				|| value is float
				|| value is double
				|| value is decimal;
		}
	}
}

