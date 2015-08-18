using System.IO;
using System.Web.Hosting;

namespace HandlebarsDotNet.Mvc.ViewEngine
{
    public class HandlebarsMvcViewEngineFileSystem : ViewEngineFileSystem
    {
        public override string GetFileContent(string filename)
        {
            var vf = HostingEnvironment.VirtualPathProvider.GetFile(filename);

            using (var stream = vf.Open())
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        protected override string CombinePath(string dir, string otherFileName)
        {
            return Path.Combine(dir, otherFileName);
        }

        public override bool FileExists(string filePath)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(filePath);
        }
    }
}