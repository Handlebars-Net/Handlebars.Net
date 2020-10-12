namespace HandlebarsDotNet.Features
{
    /// <summary>
    /// Feature allows to attach a behaviour on per-template basis by modifying template bound <see cref="HandlebarsConfiguration"/>
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Executes before any template parsing/compiling activity
        /// </summary>
        /// <param name="configuration"></param>
        void OnCompiling(ICompiledHandlebarsConfiguration configuration);

        /// <summary>
        /// Executes after template is compiled
        /// </summary>
        void CompilationCompleted();
    }
}