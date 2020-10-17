using System;
using HandlebarsDotNet.Helpers.BlockHelpers;

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
        bool TryResolveHelper(string name, Type targetType, out HelperDescriptorBase helper);

        /// <summary>
        /// Resolves <see cref="HandlebarsBlockHelper"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        bool TryResolveBlockHelper(string name, out BlockHelperDescriptorBase helper);
    }
}