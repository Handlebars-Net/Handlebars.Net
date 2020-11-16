using System;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Provides access to path expressions in the template
    /// </summary>
    public interface IPathInfoStore
    {
        PathInfo GetOrAdd(string path);
    }
    
    public class PathInfoStore : IPathInfoStore
    {
        private static readonly Lazy<PathInfoStore> Instance = new Lazy<PathInfoStore>(() => new PathInfoStore());

        private static readonly Func<string, GcDeferredValue<string, PathInfo>> ValueFactory = s =>
        {
            return new GcDeferredValue<string, PathInfo>(s, pathString =>
            {
                return PathInfo.Parse(pathString);
            });
        };
        
        public static PathInfoStore Shared => Instance.Value;

        private readonly LookupSlim<string, GcDeferredValue<string, PathInfo>, StringEqualityComparer> _paths 
            = new LookupSlim<string, GcDeferredValue<string, PathInfo>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.Ordinal));
        
        private PathInfoStore(){}
        
        public PathInfo GetOrAdd(string path)
        {
            var pathInfo = _paths.GetOrAdd(path, ValueFactory).Value;
            
            var trimmedPath = pathInfo.TrimmedPath;
            if (pathInfo.IsBlockHelper || pathInfo.IsInversion)
            {
                _paths.GetOrAdd(trimmedPath, ValueFactory);
            }

            return pathInfo;
        }
    }
}