using System.IO;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Handler called when a partial template is missing. This allows silent error handling
    /// or direct writing to the output stream.
    /// </summary>
    public interface IMissingPartialTemplateHandler
    {
        /// <summary>
        /// Called when a partial template cannot be loaded.
        /// </summary>
        /// <param name="configuration">The current environment configuration.</param>
        /// <param name="partialName">The name of the partial that was not found.</param>
        /// <param name="textWriter">The output writer.</param>
        void Handle(ICompiledHandlebarsConfiguration configuration, string partialName, TextWriter textWriter);
    }
}
