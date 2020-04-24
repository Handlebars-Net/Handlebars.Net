using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Collections;
using Xunit;

namespace Handlebars.Test
{
    public class CollectionsTests
    {
        [Fact]
        public void CascadeDictionary_OuterChanged_ValueExists()
        {
            var outer = new Dictionary<string, string>();
            var cascadeDictionary = new CascadeDictionary<string, string>(outer);
            
            outer.Add("s", "s");
            
            Assert.True(cascadeDictionary.ContainsKey("s"));
        }
        
        [Fact]
        public void CascadeDictionary_ToArray_ContainsValuesFromOuter()
        {
            var outer = new Dictionary<string, string>();
            var cascadeDictionary = new CascadeDictionary<string, string>(outer);
            
            outer.Add("s", "s");

            var array = cascadeDictionary.ToArray();
            
            Assert.Single(array);
        }
    }
}