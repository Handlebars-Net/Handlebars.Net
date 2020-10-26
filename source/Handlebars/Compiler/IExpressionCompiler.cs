using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Executes compilation of lambda <see cref="Expression{T}"/> to actual <see cref="Delegate"/> 
    /// </summary>
    public interface IExpressionCompiler
    {
        [Pure]
        T Compile<T>(Expression<T> expression) where T: class, Delegate;
    }
}