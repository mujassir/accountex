using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Configuration;
using AccountEx.Repositories.Config;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Repositories.Lab;

namespace AccountEx.Web.Controllers.mvc
{
    public class LabController : BaseController
    {
        //
        // GET: /CRM/

        public ActionResult TestCategory()
        {
            ViewBag.TestCategories = new GenericRepository<TestCategory>().GetNames();
            return View();
        }
        public ActionResult Tests()
        {

            ViewBag.TestGroup = new GenericRepository<TestCategory>().GetNames();
            var parameters = new ParameterRepository().GetAll(TestType.Pathology).OrderBy(p => p.Name).ToList();
            ViewBag.Parameters = JsonConvert.SerializeObject(parameters);
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            //ViewBag.MedicineTypes = new GenericRepository<MedicineType>().GetNames();
            return View();
        }
        public ActionResult Parameters()
        {
            ViewBag.TestGroup = new GenericRepository<TestCategory>().GetNames();
            return View();
        }








    }
}
