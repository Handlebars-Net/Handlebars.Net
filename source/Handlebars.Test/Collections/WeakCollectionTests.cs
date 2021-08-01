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
        
        private static void CallInItsOwnScope(Action scope) => scope();
    }
}