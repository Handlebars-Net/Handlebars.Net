﻿using System;
using System.IO;

namespace Handlebars
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, Action<TextWriter, object> template, dynamic context, params object[] arguments);

    public sealed class Handlebars
    {
        private static readonly IHandlebars _singleton = new HandlebarsEnvironment(new HandlebarsConfiguration());

        public static IHandlebars Create()
        {
            return new HandlebarsEnvironment(new HandlebarsConfiguration());
        }

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return _singleton.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            return _singleton.Compile(template);
        }

        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            _singleton.RegisterTemplate(templateName, template);
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            _singleton.RegisterHelper(helperName, helperFunction);
        }
    }
}

