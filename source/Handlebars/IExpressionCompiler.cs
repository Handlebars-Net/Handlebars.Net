using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Executes compilation of lambda <see cref="Expression{T}"/> to actual <see cref="Delegate"/> 
    /// </summary>
    public interface IExpressionCompiler
    {
        T Compile<T>(Expression<T> expression) where T: class;
    }
}