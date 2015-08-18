using System.Web.Mvc;

namespace HandlebarsDotNet.Mvc.Example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("post", new
            {
                post = new
                {
                    title = "My Post Title",
                    image = "/someimage.png",
                    post_class = "somepostclass"
                }
            });
        }

    }
}