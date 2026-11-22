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

        /// <summary>
        /// If <see langword="true"/> limits an inline partial (<c>{{#*inline}}</c>) declared inside a block to that block, as in Handlebarsjs.
        /// <para>The partial is visible in the declaring block and its children only; once the block has rendered it can no longer be
        /// called, and an outer partial with the same name becomes visible again:
        /// <c>{{#*inline "p"}}outer{{/inline}}{{#with .}}{{#*inline "p"}}inner{{/inline}}{{> p}}{{/with}} {{> p}}</c> renders <c>inner outer</c>.</para>
        /// <para>By default (<see langword="false"/>) the earlier Handlebars.Net behavior is kept: inline partials declared inside a block
        /// (or inside a partial) stay registered on the enclosing scope after it ends, so the template above renders <c>inner inner</c>.</para>
        /// <para>Templates that call a block's inline partial after the block will throw "could not be resolved" when this is enabled.</para>
        /// </summary>
        public bool BlockScopedInlinePartials { get; set; } = false;

        /// <summary>
        /// If <see langword="true"/> restores the earlier Handlebars.Net handling of a bare <c>{{else name}}</c>
        /// (nothing after the name) inside <c>{{#if}}</c> / <c>{{#unless}}</c>.
        /// <para>By default (<see langword="false"/>) it follows Handlebarsjs: <c>{{else name}}</c> is equivalent to
        /// <c>{{else}}{{#name}}…{{/name}}</c>, so a block helper <c>name</c> is invoked, or else <c>name</c> is resolved as a value:
        /// <c>{{#if a}}A{{else b}}B{{/if}}</c> renders <c>B</c> only when <c>a</c> is falsy and <c>b</c> is truthy, and iterates when
        /// <c>b</c> is a list. A bare <c>{{else if}}</c> / <c>{{else unless}}</c> with no condition is a <see cref="HandlebarsCompilerException"/>.</para>
        /// <para>When <see langword="true"/>, the name is ignored and the branch renders as a plain <c>{{else}}</c>:
        /// the template above renders <c>B</c> whenever <c>a</c> is falsy.</para>
        /// <para>Read at compile time. Intended only as a temporary escape hatch for templates that depend on the old behavior.</para>
        /// </summary>
        public bool LegacyBareElseNameBehavior { get; set; } = false;
    }
}