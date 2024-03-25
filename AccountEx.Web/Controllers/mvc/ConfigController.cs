using System.Collections.Generic;
using System.Web.Mvc;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using AccountEx.Web.Code;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Repositories.COA;
using Entities.CodeFirst;
using AccountEx.Repositories.Config;

namespace AccountEx.Web.Controllers.mvc
{
    [Compress]
    public class ConfigController : BaseController
    {
        //
        // GET: /Project/


        public ActionResult Models()
        {
            return View();
        }
        public ActionResult Colors()
        {
            return View();
        }
        public ActionResult EnginePowers()
        {
            return View();
        }
        public ActionResult Fuels()
        {
            return View();
        }
        public ActionResult Auctioneers()
        {
            return View();
        }
        public ActionResult CarTypes()
        {
            return View();
        }
        public ActionResult Consignees()
        {
            return View();
        }
        public ActionResult Shipeers()
        {
            return View();
        }
        public ActionResult Manufacturers()
        {
            return View();
        }
        public ActionResult ClearingCompanies()
        {
            return View();
        }
        public ActionResult Countries()
        {
            
            return View();
        }
        public ActionResult Years()
        {
            
            return View();
        }
        public ActionResult Months()
        {
           
            return View();
        }
        public ActionResult WorkingSectors()
        {
            return View();
        }
        public ActionResult Industries()
        {
            return View();
        }
        public ActionResult CostCenters() 
        {
            return View();
        }
        public ActionResult Vessels()
        {
            return View();
        }
        public ActionResult POL()
        {
            return View();
        }
        public ActionResult POD()
        {
            return View();
        }
        public ActionResult Destinations()
        {
            return View();
        }
        public ActionResult DeliveryTerms()
        {

            return View();
        }
        public ActionResult ContractTypes()
        {

            return View();
        }
    }
}
