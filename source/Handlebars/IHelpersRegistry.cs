using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.BlockHelpers;

namespace HandlebarsDotNet
{
    public interface IHelpersRegistry
    {
        IIndexed<string, IHelperDescriptor<HelperOptions>> GetHelpers();
        IIndexed<string, IHelperDescriptor<BlockHelperOptions>> GetBlockHelpers();
    }
    
    public static class HelpersRegistryExtensions
    {
        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsHelper helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetHelpers()[helperName] = new DelegateHelperDescriptor(helperName, helperFunction);
        }
        
        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsHelperWithOptions helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetHelpers()[helperName] = new DelegateHelperWithOptionsDescriptor(helperName, helperFunction);
        }
            
        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsReturnHelper helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetHelpers()[helperName] = new DelegateReturnHelperDescriptor(helperName, helperFunction);
        }
        
        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsReturnWithOptionsHelper helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetHelpers()[helperName] = new DelegateReturnHelperWithOptionsDescriptor(helperName, helperFunction);
        }
        
        public static void RegisterHelper<TRegistry>(this TRegistry registry, IHelperDescriptor<HelperOptions> helperObject)
            where TRegistry : IHelpersRegistry
        {
            registry.GetHelpers()[helperObject.Name] = helperObject;
        }

        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsBlockHelper helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetBlockHelpers()[helperName] = new DelegateBlockHelperDescriptor(helperName, helperFunction);
        }
        
        public static void RegisterHelper<TRegistry>(this TRegistry registry, string helperName, HandlebarsReturnBlockHelper helperFunction)
            where TRegistry : IHelpersRegistry
        {
            registry.GetBlockHelpers()[helperName] = new DelegateReturnBlockHelperDescriptor(helperName, helperFunction);
        }

        public static void RegisterHelper<TRegistry>(this TRegistry registry, IHelperDescriptor<BlockHelperOptions> helperObject)
            where TRegistry : IHelpersRegistry
        {
            registry.GetBlockHelpers()[helperObject.Name] = helperObject;
        }
    }
}