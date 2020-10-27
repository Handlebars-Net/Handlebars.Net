using System;

namespace HandlebarsDotNet
{
    public class FileSystemPartialTemplateResolver : IPartialTemplateResolver
    {
        public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            var handlebarsTemplateRegistrations = env.Configuration as IHandlebarsTemplateRegistrations ?? env.As<ICompiledHandlebars>().CompiledConfiguration;
            if (handlebarsTemplateRegistrations?.FileSystem == null || templatePath == null || partialName == null)
            {
                return false;
            }

            var partialPath = handlebarsTemplateRegistrations.FileSystem.Closest(templatePath,
                "partials/" + partialName + ".hbs");

            if (partialPath != null)
            {
                var compiled = env
                    .CompileView(partialPath);

                handlebarsTemplateRegistrations.RegisteredTemplates.Add(partialName, (writer, o, data) =>
                {
                    writer.Write(compiled(o, data));
                });

                return true;
            }
            else
            {
                // Failed to find partial in filesystem
                return false;
            }
        }
    }
}
