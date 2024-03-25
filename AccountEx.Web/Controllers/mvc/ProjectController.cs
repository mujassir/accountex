using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AccountEx.Repositories;

namespace AccountEx.Web.Controllers.mvc
{
    public class ProjectController : BaseController
    {
        //
        // GET: /Project/

        public ActionResult Index()
        {
            Session["rootpath"] = Server.MapPath("../");
            return View();
        }

        public string GetNextProjectNumber()
        {
            string result;
            try
            {
                var po = new ProjectRepository().GetNextProjectNumber();
                result = JsonResult(true, po + "");
            }
            catch (Exception)
            {

                result = JsonResult(false, "");
            }
            return result;
        }
        public string JsonResult(bool success, string data)
        {
            return new JavaScriptSerializer().Serialize(new
            {
                Success = success,
                Data = data,
            });
        }

    }
}
