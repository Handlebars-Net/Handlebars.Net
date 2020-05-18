using System.Diagnostics;

namespace HandlebarsDotNet.Extension.CompileFast
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompileFastExtensions
    {
        /// <summary>
        /// Changes <see cref="IExpressionCompiler"/> to the one using <c>FastExpressionCompiler</c> 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static HandlebarsConfiguration UseCompileFast(this HandlebarsConfiguration configuration)
        {
            var compileTimeConfiguration = configuration.CompileTimeConfiguration;
            
            compileTimeConfiguration.Features.Add(new FastCompilerFeatureFactory());

            return configuration;
        } 
    }
}