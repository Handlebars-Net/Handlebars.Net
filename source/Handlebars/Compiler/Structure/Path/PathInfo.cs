using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler.Structure.Path
{
    /// <summary>
    /// Represents path expression
    /// </summary>
    public class PathInfo : IEquatable<PathInfo>
    {
        private readonly string _path;
        
        internal readonly bool IsValidHelperLiteral;
        internal readonly bool HasValue;
        internal readonly bool IsThis;
        internal readonly bool IsPureThis;
        
        internal PathInfo(
            bool hasValue, 
            string path, 
            bool isValidHelperLiteral, 
            PathSegment[] segments
        )
        {
            IsValidHelperLiteral = isValidHelperLiteral;
            HasValue = hasValue;
            _path = path;

            unchecked
            {
                _hashCode = (_path.GetHashCode() * 397) ^ HasValue.GetHashCode();
            }
            
            if(!HasValue) return;

            IsVariable = path.StartsWith("@");
            IsInversion = path.StartsWith("^");
            IsBlockHelper = path.StartsWith("#");
            IsBlockClose = path.StartsWith("/");
            
            ContextChangeDepth = segments?.Count(o => o.IsContextChange) ?? 0;
            HasContextChange = ContextChangeDepth > 0;
            var plainSegments = segments?.Where(o => !o.IsContextChange && !string.IsNullOrEmpty(o.ToString())).ToArray() ?? ArrayEx.Empty<PathSegment>();
            IsThis = string.Equals(path, "this", StringComparison.OrdinalIgnoreCase) || path == "." || plainSegments.Any(o => o.IsThis);
            IsPureThis = string.Equals(path, "this", StringComparison.OrdinalIgnoreCase) || path == ".";

            var segment = plainSegments.SingleOrDefault(o => !o.IsThis);
            if (segment == null)
            {
                IsPureThis = true;
                TrimmedPath = ".";
                PathChain = ArrayEx.Empty<ChainSegment>();
                return;
            }
            
            TrimmedPath = string.Join(".", segment.PathChain.Select(o => o.TrimmedValue));
            PathChain = segment.PathChain;

            unchecked
            {
                _trimmedHashCode = TrimmedPath.GetHashCode();
            }
        }

        /// <summary>
        /// Indicates whether <see cref="PathInfo"/> is part of <c>@</c> variable
        /// </summary>
        public readonly bool IsVariable;
        
        /// <summary>
        /// 
        /// </summary>
        public readonly ChainSegment[] PathChain;

        internal readonly string TrimmedPath;
        internal readonly bool IsInversion;
        internal readonly bool IsBlockHelper;
        internal readonly bool IsBlockClose;
        internal readonly bool HasContextChange;
        internal readonly int ContextChangeDepth;
        private UndefinedBindingResult _undefinedBindingResult;
        private readonly object _lock = new object();
        private readonly int _hashCode;
        private readonly int _trimmedHashCode;
        private int _comparerTag;

        /// <summary>
        /// Used for special handling of Relaxed Helper Names
        /// </summary>
        internal void TagComparer()
        {
            _comparerTag++;
        }
        
        /// <inheritdoc />
        public bool Equals(PathInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasValue == other.HasValue && _path == other._path && _comparerTag == other._comparerTag;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PathInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => _hashCode;

        /// <summary>
        /// Returns string representation of current <see cref="PathInfo"/>
        /// </summary>
        public override string ToString() => _path;
        
        /// <inheritdoc cref="ToString"/>
        public static implicit operator string(PathInfo pathInfo) => pathInfo._path;
        
        internal UndefinedBindingResult GetUndefinedBindingResult(ICompiledHandlebarsConfiguration configuration)
        {
            if (_undefinedBindingResult != null) return _undefinedBindingResult;
            lock (_lock)
            {
                return _undefinedBindingResult ??
                       (_undefinedBindingResult = new UndefinedBindingResult(this, configuration));
            }
        }
        
        internal static IEqualityComparer<PathInfo> PlainPathComparer { get; } = new TrimmedPathEqualityComparer(false);
        internal static IEqualityComparer<PathInfo> PlainPathWithPartsCountComparer { get; } = new TrimmedPathEqualityComparer();

        private sealed class TrimmedPathEqualityComparer : IEqualityComparer<PathInfo>
        {
            private readonly bool _countParts;

            public TrimmedPathEqualityComparer(bool countParts = true)
            {
                _countParts = countParts;
            }
            
            public bool Equals(PathInfo x, PathInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                return x._comparerTag == y._comparerTag && (!_countParts || x.PathChain.Length == y.PathChain.Length) && string.Equals(x.TrimmedPath, y.TrimmedPath);
            }

            public int GetHashCode(PathInfo obj)
            {
                return obj._trimmedHashCode;
            }
        }
    }
}