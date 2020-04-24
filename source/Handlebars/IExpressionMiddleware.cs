using System.Linq.Expressions;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Allows to modify expression before lambda compilation. Should be executed as part of <see cref="IExpressionCompiler"/>.
    /// </summary>
    public interface IExpressionMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Expression Invoke(Expression expression);
    }
}