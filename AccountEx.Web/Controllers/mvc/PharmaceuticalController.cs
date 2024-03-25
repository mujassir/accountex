using AccountEx.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountEx.Web.Controllers.mvc
{
     [Compress]
    public class PharmaceuticalController : BaseController
    {
        //
        // GET: /PharmaceuticalSystem/

         public ActionResult Doctors()
        {
            return View("~/Views/Pharmaceutical/Doctors.cshtml");             
        }
         public ActionResult SalesRap()
         {
             return View("~/Views/Pharmaceutical/SalesRap.cshtml");
         }

    }
}
