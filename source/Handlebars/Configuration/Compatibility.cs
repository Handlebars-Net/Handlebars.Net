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
        
        /// <summary>
        /// If <see langword="true"/> enables support for Handlebars.Net helper naming rules.
        /// <para>This enables helper names to be not-valid Handlebars identifiers (e.g. <code>{{ one.two }}</code>)</para>
        /// <para>Such naming is not supported in Handlebarsjs and would break compatibility.</para>
        /// </summary>
        public bool RelaxedHelperNaming { get; set; } = false;
    }
}