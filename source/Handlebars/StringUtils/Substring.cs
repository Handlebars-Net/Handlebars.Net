using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.StringUtils
{
    /// <summary>
    /// Allows to perform substring-related manipulations without memory overhead
    /// </summary>
    internal readonly struct Substring
    {
        private readonly string _str;

        private readonly int _start;
        public readonly int Length;

        public static IReadOnlyList<Substring> Split(Substring str, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = new List<Substring>();
            var substringStart = 0;
            var substringLength = 0;
            for (var index = 0; index < str.Length; index++)
            {
                if (str[index] != separator)
                {
                    substringLength++;
                    continue;
                }

                var substring = new Substring(str, substringStart, substringLength);
                substringLength = 0;
                substringStart = index + 1;

                if (substring.Length != 0 || options != StringSplitOptions.RemoveEmptyEntries)
                    result.Add(substring);
            }

            if (substringLength != 0)
            {
                result.Add(new Substring(str, substringStart, substringLength));
            }
            
            return result;
        }

        public static Substring TrimStart(Substring str, char trimChar)
        {
            var start = 0;
            var length = str.Length;

            while (str[start] == trimChar)
            {
                start++;
                length--;
            }
            
            return new Substring(str, start, length);
        }
        
        public static Substring TrimEnd(Substring str, char trimChar)
        {
            var end = str.Length - 1;
            var length = str.Length;

            while (str[end] == trimChar)
            {
                end--;
                length--;
            }
            
            return new Substring(str, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Substring Trim(Substring str, char trimChar)
        {
            var substring = TrimStart(str, trimChar);
            return TrimEnd(substring, trimChar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(string str)
            :this(str, 0, str.Length)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(Substring substring)
            : this(substring._str, substring._start, substring.Length)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(Substring substring, int start)
            : this(substring._str, substring._start + start, substring.Length - start)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(Substring substring, int start, int length)
            : this(substring._str, substring._start + start, length)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(string str, int start)
            : this()
        {
            if(string.IsNullOrEmpty(str)) Throw.ArgumentNullException(nameof(str));
                
            _str = str;
            _start = start;
            Length = str!.Length - start;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(string str, int start, int length)
            : this()
        {
            if(string.IsNullOrEmpty(str)) Throw.ArgumentNullException(nameof(str));
                
            _str = str;
            _start = start;
            Length = length;
        }
        
        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(index < 0 || index >= Length) Throw.IndexOutOfRangeException();
                return _str[index + _start];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(char c) => _str[_start] == c;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(char c) => _str[_start + Length - 1] == c;

        public override string ToString() => _str.Substring(_start, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Substring other)
        {
            if (Length != other.Length) return false;
            return string.CompareOrdinal(_str, _start, other._str, other._start, Length) == 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string other)
        {
            if (Length != other?.Length) return false;
            return string.CompareOrdinal(_str, _start, other, 0, Length) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _str.GetHashCode();
                hashCode = (hashCode * 397) ^ Length;
                
                return hashCode;
            }
        }
        
        public override bool Equals(object obj) => obj is Substring other && Equals(other);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Substring a, Substring b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Substring a, Substring b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Substring a, string b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Substring a, string b) => !a.Equals(b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(string a, Substring b) => b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(string a, Substring b) => !b.Equals(a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Substring(string a) => new Substring(a);
        
        private static class Throw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void IndexOutOfRangeException(string message = null) => throw new IndexOutOfRangeException(message);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ArgumentNullException(string argument) => throw new ArgumentNullException(argument);
        }
    }
}