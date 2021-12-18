using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.StringUtils
{
    /// <summary>
    /// Allows to perform substring-related manipulations without memory overhead
    /// </summary>
    public readonly struct Substring : IEquatable<Substring>, IEquatable<string>
    {
        public readonly string String;
        public readonly int Start;
        public readonly int Length;

        public static bool EqualsIgnoreCase(Substring a, Substring b)
        {
            if (a.Length != b.Length) return false;

            for (int index = 0; index < a.Length; index++)
            {
                if (!char.ToLowerInvariant(a[index]).Equals(char.ToLowerInvariant(b[index]))) return false;
            }

            return true;
        }
        
        public static SplitEnumerator Split(Substring str, char separator, StringSplitOptions options = StringSplitOptions.None) => new SplitEnumerator(str, separator, options);

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
            : this(substring.String, substring.Start, substring.Length)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(Substring substring, int start)
            : this(substring.String, substring.Start + start, substring.Length - start)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(Substring substring, int start, int length)
            : this(substring.String, substring.Start + start, length)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(string str, int start)
            : this(str, start, str.Length - start)
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Substring(string str, int start, int length)
            : this()
        {
            if(string.IsNullOrEmpty(str)) Throw.ArgumentNullException(nameof(str));
                
            String = str;
            Start = start;
            Length = length;
        }
        
        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(index < 0 || index >= Length) Throw.IndexOutOfRangeException();
                return String[index + Start];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(char c) => String[Start] == c;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(char c) => String[Start + Length - 1] == c;

        public override string ToString() => String.Substring(Start, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SubstringEnumerator GetEnumerator() => new SubstringEnumerator(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Substring other)
        {
            if (Length != other.Length) return false;
            return string.CompareOrdinal(String, Start, other.String, other.Start, Length) == 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string other)
        {
            if (Length != other?.Length) return false;
            return string.CompareOrdinal(String, Start, other, 0, Length) == 0;
        }
        
        public override bool Equals(object obj) => obj is Substring other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (String.GetHashCode() * 397) ^ Length;
            }
        }

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
        
        public static bool Contains(Substring substring, char c)
        {
            return IndexOf(substring, c) != -1;
        }
        
        public static int IndexOf(Substring substring, char c)
        {
            return IndexOf(substring, c, 0);
        }
        
        public static int IndexOf(Substring substring, char c, int startIndex)
        {
            for (var index = startIndex; index < substring.Length; index++)
            {
                if (substring[index] == c) return index;
            }
            
            return -1;
        }
        
        public static bool IndexOf(Substring substring, char c, out int index)
        {
            return IndexOf(substring, c, 0, out index);
        }
        
        public static bool IndexOf(Substring substring, char c, int startIndex, out int index)
        {
            for (index = startIndex; index < substring.Length; index++)
            {
                if (substring[index] == c) return true;
            }
            
            return false;
        }
        
        public static int LastIndexOf(Substring substring, char c)
        {
            return LastIndexOf(substring, c, 0);
        }
        
        public static int LastIndexOf(Substring substring, char c, int startIndex)
        {
            for (var index = substring.Length - 1; index >= startIndex; index--)
            {
                if (substring[index] == c) return index;
            }
            
            return -1;
        }
        
        public static bool LastIndexOf(Substring substring, char c, out int index)
        {
            return LastIndexOf(substring, c, 0, out index);
        }
        
        public static bool LastIndexOf(Substring substring, char c, int startIndex, out int index)
        {
            for (index = substring.Length - 1; index >= startIndex; index--)
            {
                if (substring[index] == c) return true;
            }
            
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Substring(string a) => new Substring(a);
        
        public struct SubstringEnumerator : IEnumerator<char>
        {
            private readonly Substring _substring;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SubstringEnumerator(Substring substring)
            {
                _substring = substring;
                _index = -1;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _substring.Length;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset() => _index = -1;

            public char Current => _substring[_index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // nothing to do here
            }
        }
        
        public struct SplitEnumerator : IEnumerator<Substring>
        {
            private readonly Substring _substring;
            private readonly char _separator;
            private readonly StringSplitOptions _options;
            
            private Substring _current;
            private int _index;

            public SplitEnumerator(Substring substring, char separator, StringSplitOptions options = StringSplitOptions.None)
            {
                _substring = substring;
                _separator = separator;
                _options = options;
                _current = new Substring();
                _index = 0;
            }
            
            public bool MoveNext()
            {
                var substringStart = _index;
                var substringLength = 0;
                for (; _index < _substring.Length; _index++)
                {
                    if (_substring[_index] != _separator)
                    {
                        substringLength++;
                        continue;
                    }

                    var substring = new Substring(_substring, substringStart, substringLength);
                    substringLength = 0;
                    substringStart = ++_index;

                    if (substring.Length != 0 || _options != StringSplitOptions.RemoveEmptyEntries)
                    {
                        _current = substring;
                        return true;
                    }
                }

                if (substringLength != 0)
                {
                    _current = new Substring(_substring, substringStart, substringLength);
                    return true;
                }

                return false;
            }

            public void Reset() => _index = 0;

            public Substring Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // nothing to do here
            }
        }
        
        private static class Throw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void IndexOutOfRangeException(string message = null) => throw new IndexOutOfRangeException(message);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ArgumentNullException(string argument) => throw new ArgumentNullException(argument);
        }
    }
}