using System;
using System.Linq.Expressions;
using System.Collections;
using System.Linq;
using System.IO;

namespace Handlebars.Compiler
{
    internal class DeferredSectionVisitor : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new DeferredSectionVisitor(context).Visit(expr);
        }

        private readonly CompilationContext _context;

        private DeferredSectionVisitor(CompilationContext context)
        {
            _context = context;
        }

        protected override Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            Action<object, BindingContext, Action<TextWriter, object>> method;
            if(dsex.Path.Path.StartsWith("#"))
            {
                method = RenderSection;
            }
            else if(dsex.Path.Path.StartsWith("^"))
            {
                method = RenderEmptySection;
            }
            else
            {
                throw new HandlebarsCompilerException("Tried to compile a section expression that did not begin with # or ^");
            }
            return Expression.Call(
                method.Method,
                HandlebarsExpression.Path(dsex.Path.Path.Substring(1)),
                _context.BindingContext,
                new FunctionBuilder(_context.Configuration).Compile(dsex.Body, _context.BindingContext));
        }

        private static void RenderEmptySection(object value, BindingContext context, Action<TextWriter, object> template)
        {
            if(IsFalseyOrEmpty(value) == false)
            {
                throw new HandlebarsRuntimeException("Tried to render a truthy or non-empty object in an inverted section");
            }
            template(context.TextWriter, null);
        }

        private static void RenderSection(object value, BindingContext context, Action<TextWriter, object> template)
        {
            if(IsFalseyOrEmpty(value))
            {
                throw new HandlebarsRuntimeException("Tried to render a falsey or empty object in a section");
            }
            if(value is IEnumerable)
            {
                foreach(var item in ((IEnumerable)value))
                {
                    template(context.TextWriter, item);
                }
            }
            else if(value != null)
            {
                template(context.TextWriter, value);
            }
            else
            {
                throw new HandlebarsRuntimeException("Could not render value for the section");
            }
        }

        private static bool IsFalseyOrEmpty(object value)
        {
            if(value == null)
            {
                return true;
            }
            if(value is string && string.IsNullOrEmpty(value as string))
            {
                return true;
            }
            if(value is IEnumerable && ((IEnumerable)value).OfType<object>().Any() == false)
            {
                return true;
            }
            //TODO: more falsey conditions
            return false;
        }
    }
}

