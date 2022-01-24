using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}
    /// </summary>
    /// <param name="output"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate void HandlebarsHelper(EncodedTextWriter output, Context context, Arguments arguments);
    
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}
    /// </summary>
    /// <param name="output"></param>
    /// <param name="options"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate void HandlebarsHelperWithOptions(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments);
    
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}, supports <see cref="object"/> value return
    /// </summary>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate object HandlebarsReturnHelper(Context context, Arguments arguments);
    
    /// <summary>
    /// InlineHelper: {{#helper arg1 arg2}}, supports <see cref="object"/> value return
    /// </summary>
    /// <param name="options"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate object HandlebarsReturnWithOptionsHelper(in HelperOptions options, in Context context, in Arguments arguments);
    
    /// <summary>
    /// BlockHelper: {{#helper}}..{{/helper}}
    /// </summary>
    /// <param name="output"></param>
    /// <param name="options"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate void HandlebarsBlockHelper(EncodedTextWriter output, BlockHelperOptions options, Context context, Arguments arguments);
    
    /// <summary>
    /// BlockHelper: {{#helper}}..{{/helper}}
    /// </summary>
    /// <param name="options"></param>
    /// <param name="context"></param>
    /// <param name="arguments"></param>
    public delegate object HandlebarsReturnBlockHelper(BlockHelperOptions options, Context context, Arguments arguments);
    
    public delegate TemplateDelegate HandlebarsBlockDecorator(TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments);
    
    public delegate TemplateDelegate HandlebarsDecorator(TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments);
    
    public delegate void HandlebarsBlockDecoratorVoid(TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments);
    
    public delegate void HandlebarsDecoratorVoid(TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments);
}