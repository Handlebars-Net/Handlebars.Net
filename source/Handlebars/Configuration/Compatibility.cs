using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains feature flags that breaks compatibility with Handlebarsjs.
    /// </summary>
    public class Compatibility
    {
        /// <summary>
        /// If <see langword="true"/> enables support for <c>@last</c> in object properties iterations.
        /// </summary>
        [Obsolete("@last is supported on Handlebarsjs, so it is always enabled, and the setting should be removed.")]
        public bool SupportLastInObjectIterations { get; set; } = true;
        
        /// <summary>
        /// If <see langword="true"/> enables support for Handlebars.Net helper naming rules.
        /// <para>This enables helper names to be not-valid Handlebars identifiers (e.g. <code>{{ one.two }}</code>)</para>
        /// <para>Such naming is not supported in Handlebarsjs and would break compatibility.</para>
        /// </summary>
        [Obsolete("Toggle will be removed in the next major release")]
        public bool RelaxedHelperNaming { get; set; } = false;

        /// <summary>
        /// If <see langword="true"/> restores the earlier Handlebars.Net handling of a bare <c>{{^}}</c>.
        /// <para>By default (<see langword="false"/>) a bare <c>{{^}}</c> inside a block is the inverse separator,
        /// equivalent to <c>{{else}}</c>, as in Handlebarsjs: <c>{{#if c}}yes{{^}}no{{/if}}</c> renders <c>no</c> when <c>c</c> is falsy.</para>
        /// <para>When <see langword="true"/>, <c>{{^}}</c> is instead resolved as a reference to the current context
        /// and the block has no inverse, so the template above renders nothing when <c>c</c> is falsy and
        /// <c>yes</c> + the context's string value + <c>no</c> when <c>c</c> is truthy.</para>
        /// <para>Intended only as a temporary escape hatch for templates that depend on the old behavior.</para>
        /// </summary>
        public bool LegacyStandaloneCaretBehavior { get; set; } = false;
    }
}