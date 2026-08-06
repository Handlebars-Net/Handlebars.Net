using System;
using System.Diagnostics.CodeAnalysis;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Helpers
{
    /// <summary>
    /// Allows to provide helpers on-demand
    /// </summary>
    public interface IHelperResolver
    {
        /// <summary>
        /// Resolves <see cref="HandlebarsReturnHelper"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetType"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        bool TryResolveHelper(PathInfo name, Type? targetType, [NotNullWhen(true)] out IHelperDescriptor<HelperOptions>? helper);

        /// <summary>
        /// Resolves <see cref="HandlebarsBlockHelper"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        bool TryResolveBlockHelper(PathInfo name, [NotNullWhen(true)] out IHelperDescriptor<BlockHelperOptions>? helper);
    }
}