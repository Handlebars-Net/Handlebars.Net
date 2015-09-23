using System.IO;
using NUnit.Framework;

namespace HandlebarsDotNet.Test.ViewEngine
{
    [TestFixture]
    public class CasparTests
    {
        [Test]
        public void CanRenderCasparIndexTemplate()
        {
            var fs = (new DiskFileSystem());
            var handlebars = HandlebarsDotNet.Handlebars.Create(new HandlebarsConfiguration()
            {
                FileSystem = fs
            });
            AddHelpers(handlebars);
            var renderView = handlebars.CompileView("ViewEngine/Casper-master/index.hbs");
            var output = renderView(new
            {
                blog = new
                {
                    url = "http://someblog.com",
                    title = "This is the blog title"
                },
                posts = new[]
                {
                    new
                    {
                        title = "My Post Title",
                        image = "/someimage.png",
                        post_class = "somepostclass"
                    }
                }
            });
            var cq = CsQuery.CQ.CreateDocument(output);
            Assert.AreEqual("My Post Title", cq["h2.post-title a"].Text());
        }
        [Test]
        public void CanRenderCasparPostTemplate()
        {
            var fs = (new DiskFileSystem());
            var handlebars = HandlebarsDotNet.Handlebars.Create(new HandlebarsConfiguration()
            {
                FileSystem = fs
            });
            AddHelpers(handlebars);
            var renderView = handlebars.CompileView("ViewEngine/Casper-master/post.hbs");
            var output = renderView(new
            {
                blog = new
                {
                    url = "http://someblog.com",
                    title = "This is the blog title"
                },
                post = new
                {
                    title = "My Post Title",
                    image = "/someimage.png",
                    post_class = "somepostclass"
                }
            });
            var cq = CsQuery.CQ.CreateDocument(output);
            Assert.AreEqual("My Post Title", cq["h1.post-title"].Html());
        }

        private static void AddHelpers(IHandlebars handlebars)
        {
            handlebars.RegisterHelper("asset",
                (writer, context, arguments) => writer.Write("asset:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("date",
                (writer, context, arguments) => writer.Write("date:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("tags",
                (writer, context, arguments) => writer.Write("tags:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("encode",
                (writer, context, arguments) => writer.Write("encode:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("url", (writer, context, arguments) => writer.Write("url:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("excerpt", (writer, context, arguments) => writer.Write("url:" + string.Join("|", arguments)));
        }

        [Test]
        public void CanRenderCasparPostNoLayoutTemplate()
        {
            var fs = (new DiskFileSystem());
            var handlebarsConfiguration = new HandlebarsConfiguration() {FileSystem = fs};
            var handlebars = Handlebars.Create(handlebarsConfiguration);

            AddHelpers(handlebars);
            var renderView = handlebars.CompileView("ViewEngine/Casper-master/post-no-layout.hbs");
            var output = renderView(new
            {
                post = new
                {
                    title = "My Post Title",
                    image = "/someimage.png",
                    post_class = "somepostclass"
                }
            });
            var cq = CsQuery.CQ.CreateDocument(output);
            Assert.AreEqual("My Post Title", cq["h1.post-title"].Html());
        }

        class DiskFileSystem : ViewEngineFileSystem
        {
            public override string GetFileContent(string filename)
            {
                return File.ReadAllText(filename);
            }

            protected override string CombinePath(string dir, string otherFileName)
            {
                return Path.Combine(dir, otherFileName);
            }

            public override bool FileExists(string filePath)
            {
                return File.Exists(filePath);
            }
        }
    }
}
