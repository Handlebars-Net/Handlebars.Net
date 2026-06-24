using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterBinder : HandlebarsExpressionVisitor
    {
        private static readonly NewExpression NewExpression =
            (NewExpression) ((Expression<Func<HashParameterDictionary>>) (() => new HashParameterDictionary())).Body;

        private static readonly MethodInfo AddMethod =
            new Action<string, object>(new HashParameterDictionary().Add).GetMethodInfo();

        protected override Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            var elementInits = new List<ElementInit>();

            foreach (var parameter in hpex.Parameters)
            {
                elementInits.Add(Expression.ElementInit(
                    AddMethod,
                    Expression.Constant(parameter.Key),
                    Visit(parameter.Value)));
            }

            return Expression.ListInit(NewExpression, elementInits);
        }
    }
}

