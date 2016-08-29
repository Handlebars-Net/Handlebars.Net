using System;
using System.Collections;
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
		            //Expression.Call(
		            //         new Func<BindingContext, HashParametersExpression, object>(MergeContextValues).Method,
		            //         bindingContext,
		            //         Expression.Constant(pex.Argument as HashParametersExpression));

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
			// This is the only way to gain access to the template data object, at least at this particular point
			// in the compilation process.
		    var bindingContextType = typeof(BindingContext);
		    var contextValue = Expression.Property(contextExpression, bindingContextType.GetProperty("Value"));
			
		    return Expression.Call(
				new Func<object, HashParametersExpression, object>(MergeContextValues).Method,
				contextValue,
				Expression.Constant(argument)
				);
	    }

		/// <summary>
		/// Takes in an existing context value 
		/// </summary>
		/// <param name="contextValue"></param>
		/// <param name="exp"></param>
		/// <returns></returns>
	    private static object MergeContextValues(object contextValue, HashParametersExpression exp)
	    {
		    var valueDict = exp.Parameters;
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

