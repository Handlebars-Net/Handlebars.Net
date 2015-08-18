using HandlebarsDotNet.Mvc.Example;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace HandlebarsDotNet.Mvc.Example
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
