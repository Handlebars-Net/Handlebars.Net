using NUnit.Framework;
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace Handlebars.Test
{
    [TestFixture]
    public class DynamicTests
    {
        [Test]
        public void BasicDynamicObjectTest()
        {
            var model = new MyDynamicModel();

            var source = "Foo: {{foo}}\nBar: {{bar}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.AreEqual("Foo: 1\nBar: hello world", output);
        }

        private class MyDynamicModel : DynamicObject
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "foo", 1 },
                { "bar", "hello world" }
            };

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if(properties.ContainsKey(binder.Name))
                {
                    result = properties[binder.Name];
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}

