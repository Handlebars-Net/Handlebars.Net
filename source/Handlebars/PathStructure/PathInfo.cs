using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.Pools;
using HandlebarsDotNet.StringUtils;
using HandlebarsDotNet.Extensions;

namespace HandlebarsDotNet.PathStructure
{
    public enum PathType
    {
        None,
        Empty,
        Variable,
        Inversion,
        BlockHelper,
        BlockClose
    }
    
    /// <summary>
    /// Represents path expression
    /// </summary>
    public sealed partial class PathInfo : IEquatable<PathInfo>
    {
        internal readonly bool IsValidHelperLiteral;
        internal readonly bool HasValue;
        internal readonly bool IsThis;
        internal readonly bool IsPureThis;
        internal readonly bool IsInversion;
        internal readonly bool IsBlockHelper;
        internal readonly bool IsBlockClose;

        private readonly int _hashCode;
        private readonly int _trimmedHashCode;
        private readonly int _trimmedInvariantHashCode;

        public static readonly PathInfo Empty = new PathInfo(PathType.Empty, "null", false, ArrayEx.Empty<PathSegment>());
        
        private PathInfo(
            PathType pathType,
            string path,
            bool isValidHelperLiteral,
            PathSegment[] segments)
        {
            IsValidHelperLiteral = isValidHelperLiteral;
            HasValue = pathType != PathType.Empty;
            Path = path;

            _hashCode = (Path.GetHashCode() * 397) ^ HasValue.GetHashCode();
            
            if(!HasValue) return;

            IsVariable = pathType == PathType.Variable;
            IsInversion = pathType == PathType.Inversion;
            IsBlockHelper = pathType == PathType.BlockHelper;
            IsBlockClose = pathType == PathType.BlockClose;

            var plainSegments = segments.Where(o => !o.IsParent && o.IsNotEmpty).ToArray();
            IsThis = string.Equals(path, "this", StringComparison.OrdinalIgnoreCase) || path == "." || plainSegments.Any(o => o.IsThis);
            IsPureThis = string.Equals(path, "this", StringComparison.OrdinalIgnoreCase) || path == ".";
            Segments = segments;
            
            using var container = StringBuilderPool.Shared.Use();
            var builder = container.Value;
            
            var segmentsLastIndex = Segments.Length - 1;
            for (var segmentIndex = 0; segmentIndex <= segmentsLastIndex; segmentIndex++)
            {
                var segment = Segments[segmentIndex];
                var pathChainLastIndex = segment.PathChain.Length - 1;
                var pathChain = segment.PathChain;
                for (var pathChainIndex = 0; pathChainIndex <= pathChainLastIndex; pathChainIndex++)
                {
                    builder.Append(pathChain[pathChainIndex].TrimmedValue);
                    if (pathChainIndex != pathChainLastIndex) builder.Append('.');
                }

                if (segmentIndex != segmentsLastIndex) builder.Append('/');
            }
            
            TrimmedPath = builder.ToString();

            _trimmedHashCode = TrimmedPath.GetHashCode();
            _trimmedInvariantHashCode = TrimmedPath.ToLowerInvariant().GetHashCode();
        }

        /// <summary>
        /// Indicates whether <see cref="PathInfo"/> is part of <c>@</c> variable
        /// </summary>
        public readonly bool IsVariable;
        public readonly PathSegment[] Segments;
        public readonly string Path;
        public readonly string TrimmedPath;

        /// <inheritdoc />
        public bool Equals(PathInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasValue == other.HasValue && string.Equals(Path, other.Path, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is PathInfo pathInfo) return Equals(pathInfo);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => _hashCode;

        /// <summary>
        /// Returns string representation of current <see cref="PathInfo"/>
        /// </summary>
        public override string ToString() => Path;
        
        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(PathInfo pathInfo) => pathInfo.Path;
        
        public static implicit operator PathInfo(string path) => PathInfoStore.Current?.GetOrAdd(path) ?? Parse(path);
        
        public static PathInfo Parse(string path)
        {
            if (path == "null") return Empty;

            var originalPath = path;
            var pathType = GetPathType(path);
            var pathSubstring = new Substring(path);

            var isValidHelperLiteral = true;
            var isVariable = pathType == PathType.Variable;
            var isInversion = pathType == PathType.Inversion;
            var isBlockHelper = pathType == PathType.BlockHelper;
            if (isVariable || isBlockHelper || isInversion)
            {
                isValidHelperLiteral = isBlockHelper || isInversion;
                pathSubstring = new Substring(pathSubstring, 1);
            }
            
            var segments = new List<PathSegment>();
            var pathParts = Substring.Split(pathSubstring, '/');
            var extendedEnumerator = ExtendedEnumerator<Substring>.Create(pathParts);
            using var container = StringBuilderPool.Shared.Use();
            var buffer = container.Value;
            var bufferHasOpenEscapeBlock = false;
            
            while (extendedEnumerator.MoveNext())
            {
                var segment = extendedEnumerator.Current.Value;
                if (buffer.Length != 0)
                {
                    buffer.Append('/');
                    buffer.Append(in segment);
                    if(Substring.LastIndexOf(segment, ']', out var index) 
                       && !Substring.LastIndexOf(segment, '[', index, out _))
                    {
                        bufferHasOpenEscapeBlock = false;
                        var chainSegment = GetPathChain(buffer.ToString());
                        if (chainSegment.Length > 1) isValidHelperLiteral = false;

                        segments.Add(new PathSegment(segment, chainSegment));
                        buffer.Length = 0;
                        continue;
                    }
                }
                
                if(Substring.LastIndexOf(segment, '[', out var startIndex) 
                   && !Substring.LastIndexOf(segment, ']', startIndex, out _))
                {
                    buffer.Append(in segment);
                    bufferHasOpenEscapeBlock = true;
                    continue;
                }
                
                switch (segment.Length)
                {
                    case 2 when segment[0] == '.' && segment[1] == '.':
                        isValidHelperLiteral = false;
                        segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                        continue;
                    
                    case 1 when segment[0] == '.':
                        isValidHelperLiteral = false;
                        segments.Add(new PathSegment(segment, ArrayEx.Empty<ChainSegment>()));
                        continue;
                }

                var chainSegments = GetPathChain(segment);

                if (chainSegments.Length > 1 && pathType != PathType.BlockHelper) isValidHelperLiteral = false;

                if (!bufferHasOpenEscapeBlock) segments.Add(new PathSegment(segment, chainSegments));
            }

            if (isValidHelperLiteral && segments.Count > 1) isValidHelperLiteral = false;

            return new PathInfo(pathType, originalPath, isValidHelperLiteral, segments.ToArray());
        }
        
        private static ChainSegment[] GetPathChain(Substring segmentString)
        {
            var insideEscapeBlock = false;
            var pathChainParts = Substring.Split(segmentString, '.', StringSplitOptions.RemoveEmptyEntries);
            var extendedEnumerator = ExtendedEnumerator<Substring>.Create(pathChainParts);
            if (!extendedEnumerator.Any && segmentString == ".") return new[] { ChainSegment.This };
            
            var chainSegments = new List<ChainSegment>();

            while (extendedEnumerator.MoveNext())
            {
                var next = extendedEnumerator.Current.Value;
                if (insideEscapeBlock)
                {
                    if (Substring.EndsWith(next, ']'))
                    {
                        insideEscapeBlock = false;
                    }

                    chainSegments[chainSegments.Count - 1] = ChainSegment.Create($"{chainSegments[chainSegments.Count - 1]}.{next.ToString()}");
                    continue;
                }

                if (Substring.StartsWith(next, '['))
                {
                    insideEscapeBlock = true;
                }

                if (Substring.EndsWith(next,']'))
                {
                    insideEscapeBlock = false;
                }

                chainSegments.Add(ChainSegment.Create(next.ToString()));
            }

            return chainSegments.ToArray();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PathType GetPathType(string path)
        {
            return path[0] switch
            {
                '@' => PathType.Variable,
                '^' => PathType.Inversion,
                '#' => PathType.BlockHelper,
                '/' => PathType.BlockClose,
                _ => PathType.None
            };
        }
    }
}
