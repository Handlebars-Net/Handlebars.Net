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

        private DeferredSectionVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            Action<object, BindingContext, Action<TextWriter, object>> method;
            if (dsex.Path.Path.StartsWith("#"))
            {
                method = RenderSection;
            }
            else if (dsex.Path.Path.StartsWith("^"))
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
                CompilationContext.BindingContext,
                new FunctionBuilder(CompilationContext.Configuration).Compile(dsex.Body, CompilationContext.BindingContext));
        }

        private static void RenderEmptySection(object value, BindingContext context, Action<TextWriter, object> template)
        {
            if (IsFalseyOrEmpty(value) == true)
            {
                template(context.TextWriter, value);
            }
        }

        private static void RenderSection(object value, BindingContext context, Action<TextWriter, object> template)
        {
            if (IsFalseyOrEmpty(value))
            {
                return;
            }
            if (value is IEnumerable)
            {
                foreach (var item in ((IEnumerable)value))
                {
                    template(context.TextWriter, item);
                }
            }
            else if (value != null)
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
            if(IsFalsy(value))
            {
                return true;
            }
            else if (value is IEnumerable && ((IEnumerable)value).OfType<object>().Any() == false)
            {
                return true;
            }
            return false;
        }

        private static bool IsFalsy(object value)
        {
            if (value is UndefinedBindingResult)
            {
                return true;
            }
            if (value == null)
            {
                return true;
            }
            else if (value is bool)
            {
                return !(bool)value;
            }
            else if (value is string)
            {
                if ((string)value == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (IsNumber(value))
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

