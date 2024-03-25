using System.Collections.Generic;
using System.Web.Mvc;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System.Configuration;
using AccountEx.BussinessLogic;
using AccountEx.Web.Code;

namespace AccountEx.Web.Controllers.mvc
{
    //[Compress]
    public class MaintenanceController:Controller
    {
        //[PartialCache Attribute("Long")]
        public ActionResult sitemaintenance()
        {

            return View();
        }
       
        //public ActionResult Menu()
        //{
        //    return PartialView("_Menu");
        //}


        
    }
}
