using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class StatementExpression : HandlebarsExpression
    {
        private readonly Expression _body;
		private readonly bool _isEscaped;

		public StatementExpression(Expression body, bool isEscaped)
        {
            _body = body;
			_isEscaped = isEscaped;
        }

        public Expression Body
        {
            get { return _body; }
        }

		public bool IsEscaped
		{
			get { return _isEscaped; }
		}

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.StatementExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }
    }
}

