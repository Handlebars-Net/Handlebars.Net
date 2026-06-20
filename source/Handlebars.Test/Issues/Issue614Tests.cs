using System.IO;
using Xunit;

namespace HandlebarsDotNet.Test
{
    /// <summary>
    /// Tests for issue #614 — Partial indentation not preserved.
    /// When {{> partial}} is indented with spaces/tabs on its own line, every line of the
    /// rendered partial should be prefixed with that same indentation, matching Handlebars.js behavior.
    /// </summary>
    public class Issue614Tests
    {
        private readonly IHandlebars _handlebars;

        public Issue614Tests()
        {
            _handlebars = Handlebars.Create();
        }

        /// <summary>
        /// Spec section 20.12: Template "  {{> p}}" + Partial "line1\nline2" => "  line1\n  line2"
        /// The two leading spaces become the indentation for every line of the partial output.
        /// </summary>
        [Fact]
        public void InlinePartialSpecExample()
        {
            var source = "  {{> p}}";
            var partialSource = "line1\nline2";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("p", _handlebars.Compile(reader));
            }

            var result = _handlebars.Compile(source)(new { });

            Assert.Equal("  line1\n  line2", result);
        }

        /// <summary>
        /// The two-space indent before {{> user}} is applied to each line of the partial output.
        /// The trailing newline after the standalone partial invocation is stripped (standalone behaviour).
        /// </summary>
        [Fact]
        public void PartialIndentationWithMultiLinePartial()
        {
            var source = "Start\n  {{> content}}\nEnd";
            var partialSource = "line1\nline2\nline3";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("content", _handlebars.Compile(reader));
            }

            var result = _handlebars.Compile(source)(new { });

            // TrimAfter strips the \n between the partial tag and "End", so the output is:
            //   "Start\n" + "  line1\n  line2\n  line3" + "End"
            Assert.Equal("Start\n  line1\n  line2\n  line3End", result);
        }

        /// <summary>
        /// A tab character before the partial invocation is used as the indentation.
        /// </summary>
        [Fact]
        public void PartialIndentationWithTabCharacter()
        {
            var source = "Start\n\t{{> content}}\nEnd";
            var partialSource = "line1\nline2";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("content", _handlebars.Compile(reader));
            }

            var result = _handlebars.Compile(source)(new { });

            Assert.Equal("Start\n\tline1\n\tline2End", result);
        }

        /// <summary>
        /// A partial with no preceding whitespace receives no indentation.
        /// The newline that follows the standalone partial tag is stripped.
        /// </summary>
        [Fact]
        public void PartialWithNoIndentationUnchanged()
        {
            var source = "Hello\n{{> greeting}}\nBye";
            var partialSource = "World";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("greeting", _handlebars.Compile(reader));
            }

            var result = _handlebars.Compile(source)(new { });

            // Standalone with no indent: TrimAfter strips \nBye → Bye, no indent added.
            Assert.Equal("Hello\nWorldBye", result);
        }

        /// <summary>
        /// The indentation is applied inside a block helper iteration.
        /// A single-line partial produces indented output per iteration;
        /// iterations are not separated because the newline after {{> user}} is stripped.
        /// </summary>
        [Fact]
        public void PartialIndentationIsAppliedInsideBlock()
        {
            var source = "<h2>Names</h2>\n{{#names}}\n  {{> user}}\n{{/names}}";
            var partialSource = "<strong>{{name}}</strong>";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("user", _handlebars.Compile(reader));
            }

            var template = _handlebars.Compile(source);
            var data = new
            {
                names = new[]
                {
                    new { name = "Karen" },
                    new { name = "Jon" }
                }
            };

            var result = template(data);

            // The standalone \n after {{> user}} is stripped; iterations are concatenated directly.
            // Each partial invocation outputs "  <strong>Name</strong>" (indent applied).
            Assert.Equal("<h2>Names</h2>\n  <strong>Karen</strong>  <strong>Jon</strong>", result);
        }

        /// <summary>
        /// A partial whose source uses Windows-style \r\n line endings (e.g. checked out on Windows
        /// with git autocrlf=true, or produced by a StringWriter whose NewLine is \r\n) is normalised
        /// to \n in the indented output.  The library always emits \n as the line separator so that
        /// rendered output is identical across platforms.
        /// </summary>
        [Fact]
        public void PartialWithCrLfLineEndingsNormalisedToLf()
        {
            var source = "  {{> p}}";
            var partialSource = "line1\r\nline2\r\nline3";  // Windows-style \r\n

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("p", _handlebars.Compile(reader));
            }

            var result = _handlebars.Compile(source)(new { });

            // \r\n in the partial source is normalised to \n; every line gets the indent.
            Assert.Equal("  line1\n  line2\n  line3", result);
        }

        /// <summary>
        /// A multi-line partial called inside an iteration — each line of each iteration is indented.
        /// </summary>
        [Fact]
        public void MultiLinePartialIndentationInsideBlock()
        {
            var source = "{{#items}}\n  {{> row}}\n{{/items}}";
            var partialSource = "- {{name}}\n  ({{desc}})";

            using (var reader = new StringReader(partialSource))
            {
                _handlebars.RegisterTemplate("row", _handlebars.Compile(reader));
            }

            var template = _handlebars.Compile(source);
            var data = new
            {
                items = new[]
                {
                    new { name = "A", desc = "alpha" },
                    new { name = "B", desc = "beta" }
                }
            };

            var result = template(data);

            // Each 2-line partial gets "  " prepended to both lines.
            // The newline between the partial tag and the next iteration/closing tag is stripped.
            Assert.Equal("  - A\n    (alpha)  - B\n    (beta)", result);
        }

    }
}
