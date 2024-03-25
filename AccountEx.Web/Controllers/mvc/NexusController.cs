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

namespace AccountEx.Web.Controllers.mvc
{
    public class NexusController : BaseController
    {
        //
        // GET: /CRM/

        public ActionResult Cases()
        {
            ViewBag.Departments = new GenericRepository<Department>().GetNames();
            return View();
        }


        public ActionResult BulkPostedCases()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult UpdateBulkPostedCases()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult CashPostedCases()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult DepartmentMapping()
        {
            ViewBag.Departments = new AccountRepository().GetLeafAccount(SettingManager.CustomerHeadId);
            ViewBag.NexusDepartments = new NexusCaseRepository().GetReferences();
            return View();
        }
        public ActionResult InvoicePrinting()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        #region Reports
        public ActionResult SummaryOfDepartmentBilling()
        {
            return View("~/Views/Nexus/Reports/summaryofdepartmentbilling.cshtml");
        }
        public ActionResult SummaryofPending()
        {
            return View("~/Views/Nexus/Reports/summaryofpending.cshtml");
        }
        public ActionResult SummaryofDepartmentBillingByPatient()
        {

            return View("~/Views/Nexus/Reports/departmentbillingbypatient.cshtml");
        }
        public ActionResult DepartmentBillingByPatient()
        {

            return View("~/Views/Nexus/Reports/departmentbillingbypatient.cshtml");
        }
        
        public ActionResult Billingbypatient()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View("~/Views/Nexus/Reports/Billingbypatient.cshtml");
        }
        public ActionResult ReferralSummary()
        {
           
            return View("~/Views/Nexus/Reports/referralsummary.cshtml");
        }

        public ActionResult receivablesummary()
        {

            return View("~/Views/Nexus/Reports/receivablesummary.cshtml");
        }

        public ActionResult monthlyreceivablesummary()
        {

            return View("~/Views/Nexus/Reports/monthlyreceivablesummary.cshtml");
        }
        public ActionResult detailofbillpayment()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View("~/Views/Nexus/Reports/detailofbillpayment.cshtml");
        }
        public ActionResult monthlyreceiptsummary()
        {
           
            return View("~/Views/Nexus/Reports/monthlyreceiptsummary.cshtml");
        }
        #endregion










    }
}
