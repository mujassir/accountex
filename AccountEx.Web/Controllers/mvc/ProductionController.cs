using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Transactions;
using Entities.CodeFirst;
using System.Data.Entity;
using AccountEx.Repositories.Config;
using AccountEx.Web.Models;


namespace AccountEx.Web.Controllers.mvc
{

    [Compress]
    public class ProductionController : BaseController
    {

        public ActionResult ProductReceipe()
        {

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult Production()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            //setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
    }
}
