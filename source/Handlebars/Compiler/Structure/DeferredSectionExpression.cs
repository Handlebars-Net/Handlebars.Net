using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal enum SectionEvaluationMode
    {
        NonEmpty,
        Empty
    }

    internal class DeferredSectionExpression : HandlebarsExpression
    {
        private readonly PathExpression _path;
        private readonly IEnumerable<Expression> _body;
        private readonly SectionEvaluationMode _evalMode;

        public DeferredSectionExpression(
            PathExpression path,
            IEnumerable<Expression> body,
            SectionEvaluationMode evalMode)
        {
            _path = path;
            _body = body;
            _evalMode = evalMode;
        }

        public IEnumerable<Expression> Body
        {
            get { return _body; }
        }

        public PathExpression Path
        {
            get { return _path; }
        }

        public SectionEvaluationMode EvaluationMode
        {
            get { return _evalMode; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.DeferredSection; }
        }
    }
}

