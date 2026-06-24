using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HandlebarsDotNet.Test.ViewEngine
{
    public class ViewEngineTests
    {
        [Fact]
        public void CanLoadAViewWithALayout()
        {
            //Given a layout in a subfolder
            var files = new FakeFileSystem()
            {
                {"views\\somelayout.hbs", "layout start\n{{{body}}}\nlayout end"},
                //And a view in the same folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the body"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView ("views\\someview.hbs");
            var output = renderView(null);

            //Then the correct output should be rendered
            Assert.Equal("layout start\nThis is the body\nlayout end", output);
        }
        [Fact]
        public void CanLoadAWriterViewWithALayout()
        {
            //Given a layout in a subfolder
            var files = new FakeFileSystem()
            {
                {"views\\somelayout.hbs", "layout start\n{{{body}}}\nlayout end"},
                //And a view in the same folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the body"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView("views\\someview.hbs", null);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            renderView(writer, null);
            var output = sb.ToString();

            //Then the correct output should be rendered
            Assert.Equal("layout start\nThis is the body\nlayout end", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutInTheRoot()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "layout start\n{{{body}}}\nlayout end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the body"}
            };

            //When a viewengine renders that view
            var handlebars = Handlebars.Create(new HandlebarsConfiguration() {FileSystem = files});
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            //Then the correct output should be rendered
            Assert.Equal("layout start\nThis is the body\nlayout end", output);
        }

        [Fact]
        public void CanRenderInlineBlocks()
        {
            // This sample is based on https://handlebarsjs.com/examples/partials/inline-blocks.html

            var files = new FakeFileSystem()
            {
                //Given a layout in a subfolder
                { "partials/layout.hbs", "<div class=\"nav\">\n{{> nav}}\n</div>\n<div class=\"content\">\n{{> content}}\n</div>"},

                { "template.hbs", "{{#> layout}}\n{{#*inline \"nav\"}}\n{{Text}}\n{{/inline}}\n{{#*inline \"content\"}}\nMy Content\n{{/inline}}\n{{/layout}}"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView("template.hbs");
            var output = renderView(new Dictionary<string, string>
            {
                { "Text", "<My Nav>" }
            });

            //Then the correct output should be rendered
            Assert.Equal("<div class=\"nav\">\n&lt;My Nav&gt;\n</div>\n<div class=\"content\">\nMy Content\n</div>", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutWithAVariable()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "{{var1}} start\n{{{body}}}\n{{var1}} end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the {{var2}}"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView("views\\someview.hbs");
            var output = renderView(new { var1 = "layout", var2 = "body" });

            //Then the correct output should be rendered
            Assert.Equal("layout start\nThis is the body\nlayout end", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutInTheRootWithAVariable()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "{{var1}} start\n{{{body}}}\n{{var1}} end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the {{var2}}"}
            };

            //When a viewengine renders that view
            var handlebars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(new { var1 = "layout", var2 = "body" });

            //Then the correct output should be rendered
            Assert.Equal("layout start\nThis is the body\nlayout end", output);
        }

        [Fact]
        public void CanRenderAGlobalVariable()
        {
            //Given a layout in the root which contains an @ variable
            var files = new FakeFileSystem()
            {
                { "views\\someview.hbs", "This is the {{@body.title}}"}
            };

            //When a viewengine renders that view
            var handlebarsConfiguration = new HandlebarsConfiguration() {FileSystem = files};
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null, new {body = new {title = "THING"}});

            //Then the correct output should be rendered
            Assert.Equal("This is the THING", output);
        }

        [Fact]
        public void CanIgnoreCommentsContainingHtml()
        {
            var files = new FakeFileSystem()
            {
                { "views\\layout.hbs", "Start\n{{{body}}}\nEnd" },
                { "views\\someview.hbs", "{{!< layout}}\n\nTemplate\n{{!--\n<div>Commented out HTML</div>\n--}}" },
            };

            var handlebarsConfiguration = new HandlebarsConfiguration() { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            Assert.Equal("Start\n\nTemplate\n\nEnd", output);
        }

        [Fact]
        public void CanUseDictionaryModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\n\nBody: {{property}}" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(
                new Dictionary<string, object>
                {
                    { "property", "Foo" },
                }
            );

            Assert.Equal("Layout: Foo\n\nBody: Foo", output);
        }

        [Fact]
        public void CanUseDynamicModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\n\nBody: {{property}}" },
                        };

            dynamic model = new MyDynamicModel();
            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(model);

            Assert.Equal("Layout: Foo\n\nBody: Foo", output);
        }

        [Fact]
        public void CanBindToModelInNestedLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\parent_layout.hbs", "Parent layout: {{property}}\n{{{body}}}" },
                            { "views\\layout.hbs", "{{!< parent_layout}}\nLayout: {{property}}\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\n\nBody: {{property}}" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(
                new
                {
                    property = "Foo"
                }
            );

            Assert.Equal("Parent layout: Foo\nLayout: Foo\n\nBody: Foo", output);
        }

        [Fact]
        public void CanUseNullModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\n\nBody: {{property}}" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            Assert.Equal("Layout: \n\nBody: ", output);
        }

        [Fact]
        public void CanIterateOverDictionaryInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{#each this}}{{#if @First}}First:{{/if}}{{#if @Last}}Last:{{/if}}{{@Key}}={{@Value}};{{/each}}{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}} View" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration
                                          {
                                              FileSystem = files,
                                          };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(
                new Dictionary<string, object>
                {
                    { "Foo", "Bar" },
                    { "Baz", "Foo" },
                    { "Bar", "Baz" },
                }
            );

            Assert.Equal("Layout: First:Foo=Bar;Baz=Foo;Last:Bar=Baz; View", output);
        }

        [Fact]
        public void CanIterateOverObjectInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{#each this}}{{#if @First}}First:{{/if}}{{#if @Last}}Last:{{/if}}{{@Key}}={{@Value}};{{/each}}{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}} View" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration
                                          {
                                              FileSystem = files,
                                          };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(
                new
                {
                    Foo = "Bar",
                    Baz = "Foo",
                    Bar = "Baz",
                }
            );

            Assert.Equal("Layout: First:Foo=Bar;Baz=Foo;Last:Bar=Baz; View", output);
        }

        [Fact]
        public void CanIterateOverEnumerableInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{#each this}}{{#if @First}}First:{{/if}}{{#if @Last}}Last:{{/if}}{{this}};{{/each}}{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}} View" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(
                Enumerable.Range(0, 5)
            );

            Assert.Equal("Layout: First:0;1;2;3;Last:4; View", output);
        }

        [Fact]
        public void CanIterateOverNullModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{#each this}}{{#if @First}}First:{{/if}}{{#if @Last}}Last:{{/if}}{{this}};{{/each}}{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}} View" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            Assert.Equal("Layout:  View", output);
        }

        private class MyDynamicModel: DynamicObject
        {
            private readonly Dictionary<string, object> _properties =
                new Dictionary<string, object>
                {
                    { "property", "Foo" },
                };

            public override IEnumerable<string> GetDynamicMemberNames() => _properties.Keys;
            public override bool TryGetMember(GetMemberBinder binder, out object result) =>
                _properties.TryGetValue(binder.Name, out result!);
        }

        //We have a fake file system. Difference frameworks and apps will use
        //different file systems.
        class FakeFileSystem : ViewEngineFileSystem, IEnumerable
        {
            SortedDictionary<string, string> files = new SortedDictionary<string, string>(); 
            public void Add(string fileName, string fileContent)
            {
                files[Sanitise(fileName)] = fileContent;
            }

            public override string? GetFileContent(string filename)
            {
                if (!files.ContainsKey(Sanitise(filename))) return null;
                return files[Sanitise(filename)];
            }

            private static string Sanitise(string filename)
            {
                return filename.Replace("\\", "/");
            }

            protected override string CombinePath(string dir, string otherFileName)
            {
                var fullFileName = dir + "/" + otherFileName;
                fullFileName = fullFileName.TrimStart('/');
                return fullFileName;
            }

            public override bool FileExists(string filePath)
            {
                return files.ContainsKey(Sanitise(filePath));
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
