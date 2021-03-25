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
                {"views\\somelayout.hbs", "layout start\r\n{{{body}}}\r\nlayout end"},
                //And a view in the same folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the body"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView ("views\\someview.hbs");
            var output = renderView(null);

            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }
        [Fact]
        public void CanLoadAWriterViewWithALayout()
        {
            //Given a layout in a subfolder
            var files = new FakeFileSystem()
            {
                {"views\\somelayout.hbs", "layout start\r\n{{{body}}}\r\nlayout end"},
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
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutInTheRoot()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "layout start\r\n{{{body}}}\r\nlayout end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the body"}
            };

            //When a viewengine renders that view
            var handlebars = Handlebars.Create(new HandlebarsConfiguration() {FileSystem = files});
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutWithAVariable()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "{{var1}} start\r\n{{{body}}}\r\n{{var1}} end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the {{var2}}"}
            };

            //When a viewengine renders that view
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var renderView = handleBars.CompileView("views\\someview.hbs");
            var output = renderView(new { var1 = "layout", var2 = "body" });

            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
        }

        [Fact]
        public void CanLoadAViewWithALayoutInTheRootWithAVariable()
        {
            //Given a layout in the root
            var files = new FakeFileSystem()
            {
                {"somelayout.hbs", "{{var1}} start\r\n{{{body}}}\r\n{{var1}} end"},
                //And a view in a subfolder folder which uses that layout
                { "views\\someview.hbs", "{{!< somelayout}}This is the {{var2}}"}
            };

            //When a viewengine renders that view
            var handlebars = Handlebars.Create(new HandlebarsConfiguration() { FileSystem = files });
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(new { var1 = "layout", var2 = "body" });

            //Then the correct output should be rendered
            Assert.Equal("layout start\r\nThis is the body\r\nlayout end", output);
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
                { "views\\layout.hbs", "Start\r\n{{{body}}}\r\nEnd" },
                { "views\\someview.hbs", "{{!< layout}}\r\n\r\nTemplate\r\n{{!--\r\n<div>Commented out HTML</div>\r\n--}}" },
            };

            var handlebarsConfiguration = new HandlebarsConfiguration() { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            Assert.Equal("Start\r\n\r\nTemplate\r\n\r\nEnd", output);
        }

        [Fact]
        public void CanUseDictionaryModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\r\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\r\n\r\nBody: {{property}}" },
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

            Assert.Equal("Layout: Foo\r\n\r\nBody: Foo", output);
        }

        [Fact]
        public void CanUseDynamicModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\r\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\r\n\r\nBody: {{property}}" },
                        };

            dynamic model = new MyDynamicModel();
            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(model);

            Assert.Equal("Layout: Foo\r\n\r\nBody: Foo", output);
        }

        [Fact]
        public void CanBindToModelInNestedLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\parent_layout.hbs", "Parent layout: {{property}}\r\n{{{body}}}" },
                            { "views\\layout.hbs", "{{!< parent_layout}}\r\nLayout: {{property}}\r\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\r\n\r\nBody: {{property}}" },
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

            Assert.Equal("Parent layout: Foo\r\nLayout: Foo\r\n\r\nBody: Foo", output);
        }

        [Fact]
        public void CanUseNullModelInLayout()
        {
            var files = new FakeFileSystem
                        {
                            { "views\\layout.hbs", "Layout: {{property}}\r\n{{{body}}}" },
                            { "views\\someview.hbs", "{{!< layout}}\r\n\r\nBody: {{property}}" },
                        };

            var handlebarsConfiguration = new HandlebarsConfiguration { FileSystem = files };
            var handlebars = Handlebars.Create(handlebarsConfiguration);
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            Assert.Equal("Layout: \r\n\r\nBody: ", output);
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
                                              Compatibility =
                                              {
                                                  SupportLastInObjectIterations = true,
                                              },
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
                                              Compatibility =
                                              {
                                                  SupportLastInObjectIterations = true,
                                              },
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

            var handlebarsConfiguration = new HandlebarsConfiguration
                                          {
                                              FileSystem = files,
                                              Compatibility =
                                              {
                                                  SupportLastInObjectIterations = true,
                                              },
                                          };
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

            var handlebarsConfiguration = new HandlebarsConfiguration
                                          {
                                              FileSystem = files,
                                              Compatibility =
                                              {
                                                  SupportLastInObjectIterations = true,
                                              },
                                          };
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
                _properties.TryGetValue(binder.Name, out result);
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

            public override string GetFileContent(string filename)
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
