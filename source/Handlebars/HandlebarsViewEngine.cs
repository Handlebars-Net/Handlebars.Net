using System.Linq;

namespace HandlebarsDotNet
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewEngineFileSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string GetFileContent(string filename);

        private static string GetDir(string currentFilePath)
        {
            if (currentFilePath == "") return null;
            var parts = currentFilePath.Split(new[] {'\\', '/'});
            if (parts.Length == 1) return "";
            return string.Join("/", parts.Take(parts.Length - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        public string Closest(string filename, string otherFileName)
        {
            var dir = GetDir(filename);
            while (true)
            {
                if (dir == null) break;
                var fullFileName = CombinePath(dir, otherFileName);
                if (this.FileExists(fullFileName)) return fullFileName;
                dir = GetDir(dir);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract string CombinePath(string dir, string otherFileName);

        /// <summary>
        /// 
        /// </summary>
        public abstract bool FileExists(string filePath);
    }
}
