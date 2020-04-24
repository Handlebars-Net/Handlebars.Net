namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains feature flags that breaks compatibility with Handlebarsjs.
    /// </summary>
    public class Compatibility
    {
        /// <summary>
        /// If <see langword="true"/> enables support for <c>@last</c> in object properties iterations. Not supported in Handlebarsjs.
        /// </summary>
        public bool SupportLastInObjectIterations { get; set; } = false;
    }
}