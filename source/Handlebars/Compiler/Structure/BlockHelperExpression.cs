using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperExpression : HelperExpression
    {
        private readonly Expression _body;
        private readonly Expression _inversion;
        private readonly BlockParamsExpression _blockParams;

        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body,
            Expression inversion,
            bool isRaw = false)
            : this(helperName, arguments, BlockParamsExpression.Empty(), body, inversion, isRaw)
        {
            _body = body;
            _inversion = inversion;
        }
        
        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            BlockParamsExpression blockParams,
            Expression body,
            Expression inversion,
            bool isRaw = false)
            : base(helperName, arguments, isRaw)
        {
            _body = body;
            _inversion = inversion;
            _blockParams = blockParams;
        }

        public Expression Body
        {
            get { return _body; }
        }

        public Expression Inversion
        {
            get { return _inversion; }
        }

        public BlockParamsExpression BlockParams
        {
            get { return _blockParams; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.BlockExpression; }
        }
    }
}

