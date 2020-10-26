using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Mimics <see cref="Array"/> behavior however in most cases does not require memory allocation. 
    /// </summary>
    public readonly struct Arguments : IEquatable<Arguments>, IEnumerable<object>
    {
        private readonly object[] _array;
        private readonly bool _useArray;

        private readonly object _element0;
        private readonly object _element1;
        private readonly object _element2;
        private readonly object _element3;
        private readonly object _element4;
        private readonly object _element5;
        
        public readonly int Length;

        /// <summary>
        /// Ctor used to bypass struct limitations
        /// </summary>
        /// <param name="dummy">Should always by 0</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Arguments(int dummy = 0) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = null;
            _element1 = null;
            _element2 = null;
            _element3 = null;
            _element4 = null;
            _element5 = null;
            Length = dummy;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = null;
            _element2 = null;
            _element3 = null;
            _element4 = null;
            _element5 = null;
            Length = 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1, object arg2) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = arg2;
            _element2 = null;
            _element3 = null;
            _element4 = null;
            _element5 = null;
            Length = 2;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1, object arg2, object arg3) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = arg2;
            _element2 = arg3;
            _element3 = null;
            _element4 = null;
            _element5 = null;
            Length = 3;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1, object arg2, object arg3, object arg4) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = arg2;
            _element2 = arg3;
            _element3 = arg4;
            _element4 = null;
            _element5 = null;
            Length = 4;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1, object arg2, object arg3, object arg4, object arg5) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = arg2;
            _element2 = arg3;
            _element3 = arg4;
            _element4 = arg5;
            _element5 = null;
            Length = 5;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) : this()
        {
            _useArray = false;
            _array = null;
            _element0 = arg1;
            _element1 = arg2;
            _element2 = arg3;
            _element3 = arg4;
            _element4 = arg5;
            _element5 = arg6;
            Length = 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arguments(object[] args) : this()
        {
            _useArray = true;
            _array = args;
            Length = args.Length;
            
            _element0 = null;
            _element1 = null;
            _element2 = null;
            _element3 = null;
            _element4 = null;
            _element5 = null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<object> GetEnumerator() => Enumerator.Create(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary>
        /// Returns <see cref="IEnumerable{object}"/> without boxing 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> AsEnumerable() => new Enumerable(this);

        public IReadOnlyDictionary<string, object> Hash
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Length == 0) return HashParameterDictionary.Empty;
                return this[Length - 1] as HashParameterDictionary ?? HashParameterDictionary.Empty;
            }
        }

        public object this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(index < 0 || index >= Length) throw new IndexOutOfRangeException();
                
                if(_useArray) return _array[index];

                return index switch
                {
                    0 => _element0,
                    1 => _element1,
                    2 => _element2,
                    3 => _element3,
                    4 => _element4,
                    5 => _element5,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        public object this[string name]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Hash?[name];
        }
        
        [Pure]
        public Arguments Add(object value)
        {
            if (Length <= 5)
            {
                return Length switch
                {
                    0 => new Arguments(value),
                    1 => new Arguments(_element0, value),
                    2 => new Arguments(_element0, _element1, value),
                    3 => new Arguments(_element0, _element1, _element2, value),
                    4 => new Arguments(_element0, _element1, _element2, _element3, value),
                    5 => new Arguments(_element0, _element1, _element2, _element3, _element4, value),
                    _ => throw new IndexOutOfRangeException()
                };
            }

            if (!_useArray)
            {
                var array = new[]
                {
                    _element0,
                    _element1,
                    _element2,
                    _element3,
                    _element4,
                    _element5,
                    value
                };
                
                return new Arguments(array);
            }
            else
            {
                var array = new object[_array.Length + 1];
                for (var i = 0; i < _array.Length; i++)
                {
                    array[i] = _array[i];
                }

                array[_array.Length] = value;
                return new Arguments(array);
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T At<T>(in int index)
        {
            var obj = this[index];
            if (obj is null) return default;
            if (obj is T value) return value;

            var converter = TypeDescriptor.GetConverter(obj.GetType());
            return (T) converter.ConvertTo(obj, typeof(T));
        }
        
        public static implicit operator Arguments(object[] array)
        {
            return array.Length == 0 
                ? new Arguments() 
                : new Arguments(array);
        }

        public bool Equals(Arguments other)
        {
            if (_useArray && _useArray == other._useArray)
            {
                if (Length != other.Length || _array.Length != other._array.Length) return false;
                for (int i = 0; i < _array.Length; i++)
                {
                    if (!_array[i].Equals(other._array[i])) return false;
                }

                return true;
            }
        
            return Length == other.Length
                   && Equals(_element0, other._element0)
                   && Equals(_element1, other._element1)
                   && Equals(_element2, other._element2)
                   && Equals(_element3, other._element3)
                   && Equals(_element4, other._element4)
                   && Equals(_element5, other._element5);
        }
        
        public override bool Equals(object obj)
        {
            return obj is Arguments other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_array != null ? _array.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _useArray.GetHashCode();
                hashCode = (hashCode * 397) ^ (_element0 != null ? _element0.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_element1 != null ? _element1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_element2 != null ? _element2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_element3 != null ? _element3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_element4 != null ? _element4.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_element5 != null ? _element5.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Length;
                return hashCode;
            }
        }

        private sealed class Enumerable : IEnumerable<object>
        {
            private readonly Arguments _arguments;
            
            public Enumerable(in Arguments arguments) => _arguments = arguments;

            public IEnumerator<object> GetEnumerator() => Enumerator.Create(_arguments);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            
        }
        
        private sealed class Enumerator : IEnumerator<object>
        {
            private static readonly InternalObjectPool<Enumerator> Pool = new InternalObjectPool<Enumerator>(new Policy());

            public static Enumerator Create(in Arguments arguments)
            {
                var enumerator = Pool.Get();
                enumerator._index = -1;
                enumerator._arguments = arguments;

                return enumerator;
            }
            
            private Arguments _arguments;

            private int _index;
            
            private Enumerator()
            {
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => ++_index < _arguments.Length;

            public void Reset() => _index = -1;

            public object Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (_index == -1) return null;
                    if(_arguments._useArray) return _arguments._array[_index];

                    return _index switch
                    {
                        0 => _arguments._element0,
                        1 => _arguments._element1,
                        2 => _arguments._element2,
                        3 => _arguments._element3,
                        4 => _arguments._element4,
                        5 => _arguments._element5,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public void Dispose()
            {
                _arguments = new Arguments();
                Pool.Return(this);
            }
            
            private class Policy : IInternalObjectPoolPolicy<Enumerator>
            {
                public Enumerator Create() => new Enumerator();

                public bool Return(Enumerator item) => true;
            }
        }
    }
}