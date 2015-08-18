using System;
using System.Web.Mvc;

namespace HandlebarsDotNet.Mvc.ViewEngine
{
    public class HandlebarsMvcViewEngine : VirtualPathProviderViewEngine
    {
        private readonly IHandlebars _handlebars;

        public HandlebarsMvcViewEngine(IHandlebars handlebars)
        {
            _handlebars = handlebars;
            // Define the location of the View file
            this.ViewLocationFormats = new string[]
            {"~/Views/{1}/{0}.hbs", "~/Views/{0}.hbs"};

            this.PartialViewLocationFormats = new string[]
            {"~/Views/{1}/{0}.hbs", "~/Views/partials/{0}.hbs"};
        }

        protected override IView CreatePartialView
            (ControllerContext controllerContext, string partialPath)
        {
            throw new NotImplementedException();
        }

        protected override IView CreateView
            (ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new HandlebarsMvcView(viewPath, _handlebars);
        }
    }
}