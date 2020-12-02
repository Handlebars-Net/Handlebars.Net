using System;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.Runtime;

namespace HandlebarsDotNet.PathStructure
{
    public class PathInfoStore
    {
        private static readonly Func<string, DeferredValue<string, PathInfo>> ValueFactory = s =>
        {
            return new DeferredValue<string, PathInfo>(s, pathString =>
            {
                return PathInfo.Parse(pathString);
            });
        };
        
        public static PathInfoStore Current => AmbientContext.Current?.PathInfoStore;

        private readonly LookupSlim<string, DeferredValue<string, PathInfo>, StringEqualityComparer> _paths 
            = new LookupSlim<string, DeferredValue<string, PathInfo>, StringEqualityComparer>(new StringEqualityComparer(StringComparison.Ordinal));
        
        internal PathInfoStore(){}
        
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