using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using NUnit.Framework;

namespace HandlebarsDotNet.Test.ViewEngine
{
    [TestFixture]
    public class ViewEngineTests
    {
        [Test]
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
            var handleBars = Handlebars.Create(new HandlebarsConfiguration() {FileSystem = files});
            var renderView = handleBars.CompileView("views\\someview.hbs");
            var output = renderView(null);
            
            //Then the correct output should be rendered
            Assert.AreEqual("layout start\r\nThis is the body\r\nlayout end", output);
        }
        [Test]
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
            var handlebars = HandlebarsDotNet.Handlebars.Create(new HandlebarsConfiguration() {FileSystem = files});
            var render = handlebars.CompileView("views\\someview.hbs");
            var output = render(null);

            //Then the correct output should be rendered
            Assert.AreEqual("layout start\r\nThis is the body\r\nlayout end", output);
        }

        [Test]
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
            var output = render(new {@body = new {title = "THING"}});

            //Then the correct output should be rendered
            Assert.AreEqual("This is the THING", output);
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
