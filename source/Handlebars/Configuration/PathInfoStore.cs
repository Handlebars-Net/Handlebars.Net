using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Polyfills;

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
        /*
         * TODO: migrate to WeakReferences?
         */
        
        private static readonly Lazy<PathInfoStore> Instance = new Lazy<PathInfoStore>(() => new PathInfoStore());

        private static readonly Func<string, SafeDeferredValue<string, PathInfo>> ValueFactory = s =>
        {
            return new SafeDeferredValue<string, PathInfo>(s, pathString =>
            {
                return GetPathInfo(pathString);
            });
        };
        
        public static PathInfoStore Shared => Instance.Value;

        private readonly LookupSlim<string, SafeDeferredValue<string, PathInfo>> _paths = new LookupSlim<string, SafeDeferredValue<string, PathInfo>>();

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
        
        public static PathInfo GetPathInfo(string path)
        {
            if (path == "null")
                return new PathInfo(false, path, false, null);

            var originalPath = path;

            var isValidHelperLiteral = true;
            var isVariable = path.StartsWith("@");
            var isInversion = path.StartsWith("^");
            var isBlockHelper = path.StartsWith("#");
            if (isVariable || isBlockHelper || isInversion)
            {
                isValidHelperLiteral = isBlockHelper || isInversion;
                path = path.Substring(1);
            }

            var segments = new List<PathSegment>();
            var pathParts = path.Split('/');
            if (pathParts.Length > 1) isValidHelperLiteral = false;
            foreach (var segment in pathParts)
            {
                if (segment == "..")
                {
                    isValidHelperLiteral = false;
                    segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                    continue;
                }

                if (segment == ".")
                {
                    isValidHelperLiteral = false;
                    segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                    continue;
                }

                var segmentString = isVariable ? "@" + segment : segment;
                var chainSegments = GetPathChain(segmentString).ToArray();
                if (chainSegments.Length > 1) isValidHelperLiteral = false;

                segments.Add(new PathSegment(segmentString, chainSegments));
            }

            return new PathInfo(true, originalPath, isValidHelperLiteral, segments.ToArray());
        }
        
        private static IEnumerable<ChainSegment> GetPathChain(string segmentString)
        {
            var insideEscapeBlock = false;
            var pathChainParts = segmentString.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (pathChainParts.Length == 0 && segmentString == ".") return new[] {ChainSegment.Create("this")};

            var pathChain = pathChainParts.Aggregate(new List<ChainSegment>(), (list, next) =>
            {
                if (insideEscapeBlock)
                {
                    if (next.EndsWith("]"))
                    {
                        insideEscapeBlock = false;
                    }

                    list[list.Count - 1] = ChainSegment.Create($"{list[list.Count - 1]}.{next}");
                    return list;
                }

                if (next.StartsWith("["))
                {
                    insideEscapeBlock = true;
                }

                if (next.EndsWith("]"))
                {
                    insideEscapeBlock = false;
                }

                list.Add(ChainSegment.Create(next));
                return list;
            });

            return pathChain;
        }
    }
}