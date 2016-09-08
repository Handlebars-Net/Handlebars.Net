using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PartialBinder(context).Visit(expr);
        }

        private PartialBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
	        if (sex.Body is PartialExpression)
            {
                return Visit(sex.Body);
            }
	        return sex;
        }

	    protected override Expression VisitPartialExpression(PartialExpression pex)
        {
            Expression bindingContext = CompilationContext.BindingContext;
            if (pex.Argument != null)
            {
				Expression partialArg = pex.Argument;
	            if (partialArg is HashParametersExpression) // Indicates partial called with inline parameters
	            {
		            partialArg = MergeContextValues(bindingContext, (HashParametersExpression)partialArg);

	            }
		        bindingContext = Expression.Call(
			        bindingContext,
			        typeof(BindingContext).GetMethod("CreateChildContext"),
					partialArg);
            }
            return Expression.Call(
                new Action<string, BindingContext, HandlebarsConfiguration>(InvokePartial).Method,
                Expression.Convert(pex.PartialName, typeof(string)),
                bindingContext,
                Expression.Constant(CompilationContext.Configuration));
        }

	    private static Expression MergeContextValues(Expression contextExpression, HashParametersExpression argument)
	    {
		    var bindingContextType = typeof(BindingContext);
		    var contextValue = Expression.Property(contextExpression, bindingContextType.GetProperty("Value"));
			
		    return Expression.Call(
				new Func<object, object, object>(MergeContextValues).Method,
				contextValue,
				argument
				);
	    }

	    private static object MergeContextValues(object contextValue, object hashParams)
		{
			IDictionary<string, object> valueDict = new Dictionary<string, object>();
			valueDict = MergeInObject(hashParams, valueDict);
			valueDict = MergeInObject(contextValue, valueDict);

			return valueDict;
	    }

	    private static IDictionary<string, object> MergeInObject(object contextValue, IDictionary<string, object> valueDict)
	    {
		    if (contextValue == null)
		    {
			    return valueDict;
		    }
		    if (contextValue is IDictionary)
		    {
			    var contextDict = (IDictionary)contextValue;
			    foreach (var key in contextDict.Keys)
			    {
				    if (!valueDict.ContainsKey(key.ToString()))
				    {
					    valueDict.Add(key.ToString(), contextDict[key]);
				    }
			    }
		    }
		    else
		    {
			    foreach (var propertyInfo in contextValue.GetType().GetProperties())
			    {
				    if (!valueDict.ContainsKey(propertyInfo.Name))
				    {
					    valueDict.Add(propertyInfo.Name, propertyInfo.GetValue(contextValue, null));
				    }
			    }
		    }
		    return valueDict;
	    }

	    private static void InvokePartial(
            string partialName,
            BindingContext context,
            HandlebarsConfiguration configuration)
        {
            if (configuration.RegisteredTemplates.ContainsKey(partialName) == false)
            {
                if (configuration.FileSystem != null && context.TemplatePath != null)
                {
                    var partialPath = configuration.FileSystem.Closest(context.TemplatePath,
                        "partials/" + partialName + ".hbs");
                    if (partialPath != null)
                    {
                        var compiled = Handlebars.Create(configuration)
                            .CompileView(partialPath);
                        configuration.RegisteredTemplates.Add(partialName, (writer, o) =>
                        {
                            writer.Write(compiled(o));
                        });
                    }
                }
                else
                {
                    throw new HandlebarsRuntimeException(
                        string.Format("Referenced partial name {0} could not be resolved", partialName));
                }
            }
            configuration.RegisteredTemplates[partialName](context.TextWriter, context);
        }
    }
}

