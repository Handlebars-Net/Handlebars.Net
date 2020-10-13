using System;
using System.Collections.Generic;
using HandlebarsDotNet.Collections;
using Xunit;

namespace HandlebarsDotNet.Test.Collections
{
    public class FixedSizeDictionaryTests : IDisposable
    {
        public struct ObjectComparer : IEqualityComparer<object>
        {
            public bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => obj.GetHashCode();
        }
        
        private static readonly FixedSizeDictionary<object, object, ObjectComparer> FixedSizeDictionary;

        static FixedSizeDictionaryTests()
        {
            FixedSizeDictionary = new FixedSizeDictionary<object, object, ObjectComparer>(15, 17, new ObjectComparer());
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

            var destination = new FixedSizeDictionary<object, object, ObjectComparer>(15, 17, new ObjectComparer());
            FixedSizeDictionary.CopyTo(destination);
            FixedSizeDictionary.Clear();
            
            for (int i = 0; i < 245; i++)
            {
                var key = keys[i];
                var value = values[i];
                var index = indexes[i];
                
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