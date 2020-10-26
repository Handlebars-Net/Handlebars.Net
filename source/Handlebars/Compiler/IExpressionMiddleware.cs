using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Allows to modify expression before lambda compilation. Should be executed as part of <see cref="IExpressionCompiler"/>.
    /// </summary>
    public interface IExpressionMiddleware
    {
        [Pure]
        Expression<T> Invoke<T>(Expression<T> expression) where T: Delegate;
    }
}