using System;
using System.Collections.Generic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class RawHelperTests
    {
        [Fact]
        public void RawBlockHelper()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper}}}} {{foo}} {{{foo}}}{{{{otherRawBlockHelper}}}} {{ bar }}{{{bar}}}{{{{/otherRawBlockHelper}}}}{{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                options.Template(writer, null!);
            });

            var data = new
            {
                foo = "<div>foo</div>",
                bar = "<span>bar</span>",
                otherRawBlockHelper = new {
                    bar = "<div>This should definitely not render!</div>" 
                }
            };

            var template = inst.Compile(source);

            var output = template(data);
            Assert.Equal(" {{foo}} {{{foo}}}{{{{otherRawBlockHelper}}}} {{bar}}{{{bar}}}{{{{/otherRawBlockHelper}}}}", output);
        }

        [Fact]
        public void RawBlockHelperWithArguments()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper foo bar=bar}}}} {{foo}} {{{foo}}}{{{{otherRawBlockHelper}}}} {{ bar }}{{{bar}}}{{{{/otherRawBlockHelper}}}}{{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                writer.Write(arguments[0]);
                options.Template(writer, null!);
                writer.Write(((IDictionary<string, object>) arguments[1]!)["bar"]);
            });

            var data = new
            {
                foo = "foo",
                bar = "outerBar",
                otherRawBlockHelper = new
                {
                    bar = "<div>This should definitely not render!</div>"
                }
            };

            var template = inst.Compile(source);

            var output = template(data);
            Assert.Equal("foo {{foo}} {{{foo}}}{{{{otherRawBlockHelper}}}} {{bar}}{{{bar}}}{{{{/otherRawBlockHelper}}}}outerBar", output);
        }

        [Fact]
        public void HtmlIsNotEscapedInsideRawHelper()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper html}}}} {{ foo }} {{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                writer.Write(arguments[0]);
                options.Template(writer, null!);
            });

            var data = new
            {
                html = "<div>foo</div>"
            };

            var template = inst.Compile(source);
            var output = template(data);
            Assert.Equal("<div>foo</div> {{foo}} ", output);
        }

        [Fact]
        public void HtmlIsNotEscapedInsideRawHelperHashArgs()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper html foo=html}}}} {{ foo }} {{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                writer.Write(arguments[0]);
                options.Template(writer, null!);
                writer.Write(((IDictionary<string, object>) arguments[1]!)["foo"]);
            });

            var data = new
            {
                html = "<div>foo</div>"
            };

            var template = inst.Compile(source);
            var output = template(data);
            Assert.Equal("<div>foo</div> {{foo}} <div>foo</div>", output);
        }

        [Fact]
        public void RawHelperShouldNotMangleArgumentsInBody()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper}}}}{{someHelper fooArg fooHashArg='foo' fooHashArgDoubleQuote=\"foo!\" barHashArg=unquotedValue bazHashArg=@root.baz.nested}}{{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                options.Template(writer, null!);
            });

            var template = inst.Compile(source);
            var output = template(new { });

            Assert.Equal("{{someHelper fooArg fooHashArg='foo' fooHashArgDoubleQuote=\"foo!\" barHashArg=unquotedValue bazHashArg=@root.baz.nested}}", output);
        }

        [Fact]
        public void RawHelperShouldNotMangleArgumentsInBodyIfAnExistingHelperIsReferenced()
        {
            var inst = Handlebars.Create();
            var source = "{{{{rawBlockHelper}}}}{{someHelper fooArg fooHashArg='foo' fooHashArgDoubleQuote=\"foo!\" barHashArg=unquotedValue bazHashArg=@root.baz.nested}}{{{{/rawBlockHelper}}}}";

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                options.Template(writer, null!);
            });

            inst.RegisterHelper("someHelper", (writer, context, parameters) =>
            {
                throw new Exception("If this gets called, something went terribly wrong.");
            });

            var template = inst.Compile(source);
            var output = template(new { });

            Assert.Equal("{{someHelper fooArg fooHashArg='foo' fooHashArgDoubleQuote=\"foo!\" barHashArg=unquotedValue bazHashArg=@root.baz.nested}}", output);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/676
        [Theory]
        [InlineData(false, "{{{{raw}}}}{{{{/raw}}}}", "")]
        [InlineData(true, "{{{{raw}}}}{{{{/raw}}}}", "")]
        [InlineData(false, "{{{{raw}}}}a{{foo}}b{{{{/raw}}}}", "a{{foo}}b")]
        [InlineData(true, "{{{{raw}}}}a{{foo}}b{{{{/raw}}}}", "a{{foo}}b")]
        [InlineData(false, "{{{{raw}}}}{{{{other}}}}x{{{{/other}}}}{{{{/raw}}}}", "{{{{other}}}}x{{{{/other}}}}")]
        [InlineData(true, "{{{{raw}}}}{{{{other}}}}x{{{{/other}}}}{{{{/raw}}}}", "{{{{other}}}}x{{{{/other}}}}")]
        [InlineData(false, "{{{{raw foo}}}}-{{bar}}-{{{{/raw}}}}", "1-{{bar}}-")]
        [InlineData(true, "{{{{raw foo}}}}-{{bar}}-{{{{/raw}}}}", "1-{{bar}}-")]
        public void RawBlockHelperCompilesRegardlessOfRelaxedHelperNaming(bool relaxedHelperNaming, string source, string expected)
        {
            var inst = Handlebars.Create();
#pragma warning disable CS0618
            inst.Configuration.Compatibility.RelaxedHelperNaming = relaxedHelperNaming;
#pragma warning restore CS0618
            inst.RegisterHelper("raw", (writer, options, context, arguments) =>
            {
                if (arguments.Length > 0) writer.Write(arguments[0]);
                options.Template(writer, context);
            });

            var template = inst.Compile(source);

            Assert.Equal(expected, template(new { foo = 1, bar = 2 }));
        }

        [Fact]
        public void TestNonClosingRawBlockExpressionException()
        {
            var inst = Handlebars.Create();

            inst.RegisterHelper("rawBlockHelper", (writer, options, context, arguments) => {
                writer.Write(arguments[0]);
                options.Template(writer, null!);
                writer.Write(((IDictionary<string, object>) arguments[1]!)["bar"]);
            });

            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                inst.Compile("{{{{rawBlockHelper}}}}{{foo}}")(new { foo = "foo" });
            });
        }

        [Fact]
        public void TestMissingRawHelperRawBlockExpressionException()
        {
            var inst = Handlebars.Create();

            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                inst.Compile("{{{{rawBlockHelper}}}}{{foo}}{{{{/rawBlockHelper}}}}")(new { foo = "foo" });
            });
        }
    }
}

