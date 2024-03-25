using System.Web.Mvc;

namespace AccountEx.Web.Controllers.mvc
{
    public class MetronicController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult BasicDT()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResponsiveDT()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ManagedDT()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult EditableDT()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AdvancedDT()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AjaxDT()
        {
            return View();
        }

      
    }
}
