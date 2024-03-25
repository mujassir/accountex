using System.Web.Mvc;

namespace AccountEx.Web.Controllers.mvc
{
    public class ErrorPagesController : Controller
    {
        //
        // GET: /ErrorPages/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error404()
        {
            return View();
        }
        public ActionResult Error500()
        {
            return View();
        }
        public ActionResult Oops()
        {
            return View();
        }

    }
}
