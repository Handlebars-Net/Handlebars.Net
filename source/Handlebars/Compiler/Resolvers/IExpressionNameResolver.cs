namespace HandlebarsDotNet.Compiler.Resolvers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExpressionNameResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expressionName"></param>
        /// <returns></returns>
        string ResolveExpressionName(object instance, string expressionName);
    }
}