using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HashParameterBinder(context).Visit(expr);
        }

        private HashParameterBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            var kvpConstructor = typeof(KeyValuePair<string, object>).GetConstructor(new[] { typeof(string), typeof(object) });

            var items = hpex.Parameters
                .Select(kvp =>
                    Expression.New(kvpConstructor,
                        Expression.Constant(kvp.Key), Expression.Convert((Expression)kvp.Value, typeof(object))));

            return Expression.Call(
#if netstandard
                new Func<IEnumerable<KeyValuePair<string, object>>, HashParameterDictionary>(CreateParameters).GetMethodInfo(),
#else
                new Func<IEnumerable<KeyValuePair<string, object>>, HashParameterDictionary>(CreateParameters).Method,
#endif
                Expression.NewArrayInit(typeof(KeyValuePair<string, object>), items));
        }

        private static HashParameterDictionary CreateParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var result = new HashParameterDictionary();
            foreach (var parameter in parameters)
            {
                result.Add(parameter.Key, parameter.Value);
            }
            return result;
        }
    }
}

