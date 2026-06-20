using System;
using System.Linq;
using HandlebarsDotNet.Collections;
using Xunit;

namespace HandlebarsDotNet.Test.Collections
{
    public class WeakCollectionTests
    {
        private readonly WeakCollection<object> _collection = new WeakCollection<object>();

        [Fact]
        public void ObjectIsRemovedFromCollectionOnceNotReachable()
        {
            var _0 = new object();
            _collection.Add(_0);
            
            CallInItsOwnScope(() =>
            {
                var _1 = new object();
                _collection.Add(_1);
            
                GC.Collect();
                Assert.Equal(2, _collection.Size);
                Assert.Equal(2, _collection.Count());
            });

            GC.Collect();
            Assert.Equal(2, _collection.Size);
            Assert.Single(_collection);
        }
        
        [Fact]
        public void BucketInCollectionReused()
        {
            var _0 = new object();
            _collection.Add(_0);
            
            CallInItsOwnScope(() =>
            {
                _collection.Add(new object());
                _collection.Add(new object());
                _collection.Add(new object());
            });

            GC.Collect();

            _ = _collection.Count(); // force enumeration to set inner index
            
            var _2 = new object();
            var _3 = new object();
            _collection.Add(_2);
            _collection.Add(_3);
            
            GC.Collect();
            Assert.Equal(4, _collection.Size);
            Assert.Equal(3, _collection.Count());
        }
        
        [Fact]
        public void BucketInCollectionReusedAfterRemove()
        {
            var _0 = new object();
            var _1 = new object();
            var _2 = new object();
            _collection.Add(_0);
            _collection.Add(_1);
            _collection.Add(_2);
            
            GC.Collect();
            
            _collection.Remove(_0);
            _collection.Remove(_1);
            _collection.Add(new object());
            
            Assert.Equal(3, _collection.Size);
            Assert.Equal(2, _collection.Count());
        }
        
        // Reproduces the WeakCollection growth described in PR #604.
        // WeakCollection.Add() only resets _firstAvailableIndex during Remove() or
        // enumeration. In the static Handlebars.Compile() path each compilation
        // registers new FormatterProvider/ObjectDescriptorFactory observers via Add()
        // but never enumerates the collection. After GC those observer slots become
        // dead WeakReferences, but _firstAvailableIndex is already past them, so
        // every subsequent Add() scans to the end and appends — O(N²) total work.
        [Fact]
        public void BucketInCollectionReused_WithoutRequiringEnumeration()
        {
            CallInItsOwnScope(() =>
            {
                _collection.Add(new object());
                _collection.Add(new object());
                _collection.Add(new object());
            });

            GC.Collect();

            // Dead slots must be reusable without first enumerating the collection.
            // Bug:  _firstAvailableIndex == 3; Add() starts scanning there, finds
            //       nothing before the end, and appends → Size grows to 5.
            // Fix:  Add() resets to scan from 0 when it reaches the list end,
            //       finds dead slots at [0..2], and reuses them → Size stays at 3.
            var _2 = new object();
            var _3 = new object();
            _collection.Add(_2);
            _collection.Add(_3);

            Assert.Equal(3, _collection.Size);
            Assert.Equal(2, _collection.Count());
            GC.KeepAlive(_2);
            GC.KeepAlive(_3);
        }

        private static void CallInItsOwnScope(Action scope) => scope();
    }
}