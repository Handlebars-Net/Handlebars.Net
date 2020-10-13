using System.Collections;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Provides access to path expressions in the template
    /// </summary>
    public interface IPathInfoStore : IReadOnlyDictionary<string, PathInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        PathInfo GetOrAdd(string path);
    }
    
    internal class PathInfoStore : IPathInfoStore
    {
        private readonly Dictionary<string, PathInfo> _paths = new Dictionary<string, PathInfo>();
        
        public PathInfo GetOrAdd(string path)
        {
            if (_paths.TryGetValue(path, out var pathInfo)) return pathInfo;
            
            pathInfo = PathResolver.GetPathInfo(path);
            _paths.Add(path, pathInfo);
            
            var trimmedPath = pathInfo.TrimmedPath;
            if ((pathInfo.IsBlockHelper || pathInfo.IsInversion) && !_paths.ContainsKey(trimmedPath))
            {
                _paths.Add(trimmedPath, PathResolver.GetPathInfo(trimmedPath));
            }

            return pathInfo;
        }

        public IEnumerator<KeyValuePair<string, PathInfo>> GetEnumerator() => _paths.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => _paths.GetEnumerator();

        int IReadOnlyCollection<KeyValuePair<string, PathInfo>>.Count => _paths.Count;

        bool IReadOnlyDictionary<string, PathInfo>.ContainsKey(string key) => _paths.ContainsKey(key);

        public bool TryGetValue(string key, out PathInfo value) => _paths.TryGetValue(key, out value);

        public PathInfo this[string key] => _paths[key];

        public IEnumerable<string> Keys => _paths.Keys;

        public IEnumerable<PathInfo> Values => _paths.Values;
    }
}