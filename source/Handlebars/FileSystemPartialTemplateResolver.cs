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

            if (env.Configuration?.FileSystem == null || templatePath == null || partialName == null)
            {
                return false;
            }

            var partialPath = env.Configuration.FileSystem.Closest(templatePath,
                "partials/" + partialName + ".hbs");

            if (partialPath != null)
            {
                var compiled = env
                    .CompileView(partialPath);

                env.Configuration.RegisteredTemplates.Add(partialName, (writer, o) =>
                {
                    writer.Write(compiled(o));
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
