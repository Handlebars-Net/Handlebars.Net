using System;
using System.IO;
using Handlebars.Compiler;
using System.Text;

namespace Handlebars
{
	public interface IHandlebars
	{
        Action<TextWriter, object> Compile(TextReader template);

        Func<object, string> Compile(string template);

        void RegisterTemplate(string templateName, Action<TextWriter, object> template);

        void RegisterHelper(string helperName, HandlebarsHelper helperFunction);

        void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction);
	}
}

