using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Features;

namespace HandlebarsDotNet.Test
{
    public class HandlebarsEnvGenerator : IEnumerable<object[]>
    {
        private readonly List<IHandlebars> _data = new()
        {
            Handlebars.Create(),
            Handlebars.Create(new HandlebarsConfiguration().Configure(o => o.Compatibility.RelaxedHelperNaming = true)),
            Handlebars.Create(new HandlebarsConfiguration().UseWarmUp(types =>
            {
                types.Add(typeof(Dictionary<string, object>));
                types.Add(typeof(Dictionary<int, object>));
                types.Add(typeof(Dictionary<long, object>));
                types.Add(typeof(Dictionary<string, string>));
            })),
            Handlebars.Create(new HandlebarsConfiguration().Configure(o => o.TextEncoder = new HtmlEncoder())),
        };

        public IEnumerator<object[]> GetEnumerator() => _data.Select(o => new object[] { o }).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}