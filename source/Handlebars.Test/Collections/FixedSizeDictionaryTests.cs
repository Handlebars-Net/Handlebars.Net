using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using Xunit;

namespace HandlebarsDotNet.Test.Collections
{
    public class FixedSizeDictionaryTests : IDisposable
    {
        private static readonly FixedSizeDictionary<object, object, EqualityComparers.ReferenceEqualityComparer<object>> FixedSizeDictionary;

        static FixedSizeDictionaryTests()
        {
            var referenceEqualityComparer = new EqualityComparers.ReferenceEqualityComparer<object>();
            FixedSizeDictionary = new FixedSizeDictionary<object, object, EqualityComparers.ReferenceEqualityComparer<object>>(15, 17, referenceEqualityComparer);
        }

        [Fact]
        public void AddOrReplace_Collisions()
        {
            var comparer = new CollisionsComparer(new Random().Next());
            var dictionary = new FixedSizeDictionary<object, object, CollisionsComparer>(16, 7, comparer);
            for (var i = 0; i < dictionary.Capacity; i++)
            {
                dictionary.AddOrReplace(new object(), new object(), out _);
            }
        }
        
        private readonly struct CollisionsComparer : IEqualityComparer<object>
        {
            private readonly int _hash;

            public CollisionsComparer(int hash)
            {
                _hash = hash;
            }
            
            public bool Equals(object x, object y) => false;

            public int GetHashCode(object obj) => _hash;
        }

        [Fact]
        public void AddOrReplace()
        {
            var objects = new object[245];
            for (int i = 0; i < 245; i++)
            {
                objects[i] = new object();
            }

            for (int iteration = 0; iteration < 5; iteration++)
            {
                for (int i = 0; i < 245; i++)
                {
                    var key = objects[i];
                    var value = new object();

                    FixedSizeDictionary.AddOrReplace(key, value, out var index);
                    
                    Assert.True(FixedSizeDictionary.ContainsKey(index));
                    Assert.True(FixedSizeDictionary.ContainsKey(key));
                    Assert.True(FixedSizeDictionary.TryGetValue(key, out var actualValue));
                
                    Assert.Equal(value, actualValue);
                
                    Assert.True(FixedSizeDictionary.TryGetValue(index, out actualValue));
                
                    Assert.Equal(value, actualValue);
                }
            }
        }
        
        [Fact]
        public void Add()
        {
            for (int i = 0; i < 245; i++)
            {
                var key = new object();
                var value = new object();

                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                
                Assert.True(FixedSizeDictionary.ContainsKey(index));
                Assert.True(FixedSizeDictionary.ContainsKey(key));
                Assert.True(FixedSizeDictionary.TryGetValue(key, out var actualValue));
                
                Assert.Equal(value, actualValue);
                
                Assert.True(FixedSizeDictionary.TryGetValue(index, out actualValue));
                
                Assert.Equal(value, actualValue);
            }
        }
        
        [Fact]
        public void Copy()
        {
            var keys = new object[245];
            var values = new object[245];
            for (int i = 0; i < 245; i++)
            {
                keys[i] = new object();
                values[i] = new object();
            }
            
            var indexes = new EntryIndex<object>[245];
            for (int i = 0; i < 245; i++)
            {
                var key = keys[i];
                var value = values[i];

                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                indexes[i] = index;
                
                Assert.True(FixedSizeDictionary.ContainsKey(index));
                Assert.True(FixedSizeDictionary.ContainsKey(key));
                Assert.True(FixedSizeDictionary.TryGetValue(key, out var actualValue));
                
                Assert.Equal(value, actualValue);
                
                Assert.True(FixedSizeDictionary.TryGetValue(index, out actualValue));
                
                Assert.Equal(value, actualValue);
            }

            var destination = new FixedSizeDictionary<object, object, EqualityComparers.ReferenceEqualityComparer<object>>(15, 17, new EqualityComparers.ReferenceEqualityComparer<object>());
            FixedSizeDictionary.CopyTo(destination);
            FixedSizeDictionary.Clear();
            
            for (int i = 0; i < 245; i++)
            {
                var key = keys[i];
                var value = values[i];
                destination.TryGetIndex(key, out var index);
                
                Assert.True(destination.ContainsKey(index));
                Assert.True(destination.ContainsKey(key));
                Assert.True(destination.TryGetValue(key, out var actualValue));
                
                Assert.Equal(value, actualValue);
                
                Assert.True(destination.TryGetValue(index, out actualValue));
                
                Assert.Equal(value, actualValue);
            }
        }
        
        [Fact]
        public void ResetClears()
        {
            var objects = new object[245];
            for (var i = 0; i < 245; i++)
            {
                objects[i] = new object();
            }
            
            var indexes = new EntryIndex<object>[245];
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();

                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                indexes[i] = index;
            }
            
            FixedSizeDictionary.Reset();
            
            for (var i = 0; i < 245; i++)
            {
                Assert.False(FixedSizeDictionary.ContainsKey(objects[i]));
                Assert.False(FixedSizeDictionary.ContainsKey(indexes[i]));
                
                Assert.False(FixedSizeDictionary.TryGetValue(objects[i], out _));
                Assert.False(FixedSizeDictionary.TryGetValue(indexes[i], out _));
            }
        }
        
        [Fact]
        public void CanAddAfterReset()
        {
            var objects = new object[245];
            for (var i = 0; i < 245; i++)
            {
                objects[i] = new object();
            }
            
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();

                FixedSizeDictionary.AddOrReplace(key, value, out _);
            }
            
            FixedSizeDictionary.Reset();
            
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();
                
                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                
                Assert.True(FixedSizeDictionary.ContainsKey(index));
                Assert.True(FixedSizeDictionary.ContainsKey(key));
                Assert.True(FixedSizeDictionary.TryGetValue(key, out var actualValue));
                
                Assert.Equal(value, actualValue);
                
                Assert.True(FixedSizeDictionary.TryGetValue(index, out actualValue));
                
                Assert.Equal(value, actualValue);
            }
        }
        
        [Fact]
        public void ClearClears()
        {
            var objects = new object[245];
            for (var i = 0; i < 245; i++)
            {
                objects[i] = new object();
            }
            
            var indexes = new EntryIndex<object>[245];
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();

                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                indexes[i] = index;
            }
            
            FixedSizeDictionary.Clear();
            
            for (var i = 0; i < 245; i++)
            {
                Assert.False(FixedSizeDictionary.ContainsKey(objects[i]));
                Assert.False(FixedSizeDictionary.ContainsKey(indexes[i]));
                
                Assert.False(FixedSizeDictionary.TryGetValue(objects[i], out _));
                Assert.False(FixedSizeDictionary.TryGetValue(indexes[i], out _));
            }
        }
        
        [Fact]
        public void CanAddAfterClear()
        {
            var objects = new object[245];
            for (var i = 0; i < 245; i++)
            {
                objects[i] = new object();
            }
            
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();

                FixedSizeDictionary.AddOrReplace(key, value, out _);
            }
            
            FixedSizeDictionary.Clear();
            
            for (var i = 0; i < 245; i++)
            {
                var key = objects[i];
                var value = new object();
                
                FixedSizeDictionary.AddOrReplace(key, value, out var index);
                
                Assert.True(FixedSizeDictionary.ContainsKey(index));
                Assert.True(FixedSizeDictionary.ContainsKey(key));
                Assert.True(FixedSizeDictionary.TryGetValue(key, out var actualValue));
                
                Assert.Equal(value, actualValue);
                
                Assert.True(FixedSizeDictionary.TryGetValue(index, out actualValue));
                
                Assert.Equal(value, actualValue);
            }
        }

        public void Dispose()
        {
            FixedSizeDictionary.OptionalClear();
        }
    }
}