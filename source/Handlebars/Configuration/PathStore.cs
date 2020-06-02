using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    internal class PathStore
    {
        private readonly Dictionary<string, PathInfo> _paths = new Dictionary<string, PathInfo>();
        
        public PathInfo GetOrAdd(string path)
        {
            if (_paths.TryGetValue(path, out var pathInfo)) return pathInfo;
            
            pathInfo = PathResolver.GetPathInfo(path);
            _paths.Add(path, pathInfo);

            return pathInfo;
        }
    }
}