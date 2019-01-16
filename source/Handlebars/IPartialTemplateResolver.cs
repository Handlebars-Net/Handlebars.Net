namespace HandlebarsDotNet
{
    /// <summary>
    /// Template resolver that gets called when an unknown partial is requested.
    /// </summary>
    public interface IPartialTemplateResolver
    {
        /// <summary>
        /// Attempt to get and register a partial template.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="partialName">The name of the partial to load.</param>
        /// <param name="templatePath"></param>
        /// <returns>True if the partial was found and loaded successfully. Otherwise false.</returns>
        bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath);
    }
}
