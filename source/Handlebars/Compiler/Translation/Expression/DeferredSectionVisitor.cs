using System;
using System.Linq.Expressions;
using System.Collections;
using System.Linq;
using System.IO;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class DeferredSectionVisitor : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new DeferredSectionVisitor(context).Visit(expr);
        }

        private DeferredSectionVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            var templates = GetDeferredSectionTemplates(dsex);
            
            var path = ExpressionShortcuts.Arg<object>(HandlebarsExpression.Path(dsex.Path.Path.Substring(1)));
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var body = ExpressionShortcuts.Arg(templates[0]);
            var inversion = ExpressionShortcuts.Arg(templates[1]);

            return ExpressionShortcuts.Call(() => RenderSection(path, context, body, inversion));
        }

        private Expression<Action<TextWriter, object>>[] GetDeferredSectionTemplates(DeferredSectionExpression dsex)
        {
            var fb = new FunctionBuilder(CompilationContext.Configuration);
            var body = fb.Compile(dsex.Body.Expressions, CompilationContext.BindingContext);
            var inversion = fb.Compile(dsex.Inversion.Expressions, CompilationContext.BindingContext);

            var sectionPrefix = dsex.Path.Path.Substring(0, 1);

            switch (sectionPrefix)
            {
                case "#":
                    return new[] {body, inversion};
                case "^":
                    return new[] {inversion, body};
                default:
                    throw new HandlebarsCompilerException("Tried to compile a section expression that did not begin with # or ^");
            }
        }

        private static void RenderSection(object value, BindingContext context, Action<TextWriter, object> body, Action<TextWriter, object> inversion)
        {
            var boolValue = value as bool?;
            var enumerable = value as IEnumerable;

            if (boolValue == true)
            {
                body(context.TextWriter, context);
            }
            else if (boolValue == false)
            {
                inversion(context.TextWriter, context);
            }
            else if (HandlebarsUtils.IsFalsyOrEmpty(value))
            {
                inversion(context.TextWriter, context);
            }
            else if (value is string)
            {
                body(context.TextWriter, value);
            }
            else if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    body(context.TextWriter, item);
                }
            }
            else
            {
                body(context.TextWriter, value);
            }
        }
    }
}

