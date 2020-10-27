using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class EncodedTextWriterTests
    {
        public class DataGenerator : IEnumerable<object[]>
        {
            private readonly List<object> _data = new List<object>
            {
                1, 1F, 1D, 1L, (short)1, "1", UndefinedBindingResult.Create("undefined"), true, false, 1U, (ushort)1, 1UL, '1', (decimal)1
            };

            public IEnumerator<object[]> GetEnumerator() => _data.Select(o => new object[] { o }).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        [Theory]
        [ClassData(typeof(DataGenerator))]
        public void Write(object value)
        {
            var stringWriter = new StringWriter();
            static string Formatter(UndefinedBindingResult undefined) => undefined.ToString();
            
            using var writer = new EncodedTextWriter(stringWriter, null, Formatter);
            
            writer.Write(value);
            
            Assert.Equal(value.ToString(), stringWriter.ToString());
        }
    }
}