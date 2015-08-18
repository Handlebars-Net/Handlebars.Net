using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HandlebarsDotNet;
using HandlebarsDotNet.Mvc.ViewEngine;
using Handlebars = HandlebarsDotNet.Handlebars;

namespace HandlebarsDotNet.Mvc.Example
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            RegisterHandlebarsViewEngine();
        }

        private static void RegisterHandlebarsViewEngine()
        {
            ViewEngines.Engines.Clear();

            HandlebarsConfiguration config = new HandlebarsConfiguration
            {
                FileSystem = new HandlebarsMvcViewEngineFileSystem(),
            };
            var handlebars = HandlebarsDotNet.Handlebars.Create(config);

            /* Helpers need to be registered up front - these are dummmy implementations of the ones used in Ghost*/
            handlebars.RegisterHelper("asset", (writer, context, arguments) => writer.Write("asset:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("date", (writer, context, arguments) => writer.Write("date:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("tags", (writer, context, arguments) => writer.Write("tags:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("encode", (writer, context, arguments) => writer.Write("encode:" + string.Join("|", arguments)));
            handlebars.RegisterHelper("url", (writer, context, arguments) => writer.Write("url:" + string.Join("|", arguments)));

            ViewEngines.Engines.Add(new HandlebarsMvcViewEngine(handlebars));
        }
    }
}
