namespace Handlebars.Compiler.Resolvers
{
    public interface IExpressionNameResolver
    {
        string ResolveExpressionName(string expressionName);
    }
}