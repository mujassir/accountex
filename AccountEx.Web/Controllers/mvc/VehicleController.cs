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
using System;
using AccountEx.Web.Models;

namespace AccountEx.Web.Controllers.mvc
{
    [Compress]
    public class VehicleController : BaseController
    {
        public ActionResult UserBranches()
        {
            ViewBag.Users = new UserRepository().GetNames("Username");
            ViewBag.Branches = new VehicleBranchRepository().GetNames();
            return View();

        }
        public ActionResult VehicleRecovery()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            //ViewBag.Auctioneers = new AcutionerRepository().GetNames();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }

        public ActionResult VehicleFollowUp()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult SaleComparisionByMonthYear()
        {

            return View("~/Views/Vehicle/Reports/SaleComparisionByMonthYear.cshtml");

        }
        public ActionResult SaleRegister()
        {

            return View("~/Views/Vehicle/Reports/SaleRegister.cshtml");

        }
        public ActionResult SaleRegisterProductWise()
        {

            return View("~/Views/Vehicle/Reports/SaleRegisterProductWise.cshtml");

        }
        public ActionResult ProfitLoss()
        {
            var setting = new List<FormSetting>();
            return View("~/Views/Vehicle/Reports/ProfitLoss.cshtml");
        }
        public ActionResult CustomerBalances()
        {

            return View("~/Views/Vehicle/Reports/CustomerBalances.cshtml");

        }
        public ActionResult CashBankSummary()
        {

            return View("~/Views/Vehicle/Reports/cashbanksummary.cshtml");

        }
        public ActionResult IncomeStatement()
        {

            return View("~/Views/Vehicle/Reports/IncomeStatement.cshtml");

        }
        public ActionResult PeriodicActivity()
        {
            ViewBag.Accounts = new AccountRepository().GetByLevel(3);
            return View("~/Views/Vehicle/Reports/PeriodicActivity.cshtml");

        }
        public ActionResult Followups()
        {
            ViewBag.Accounts = new AccountRepository().GetByLevel(3);
            return View("~/Views/Vehicle/Reports/Followups.cshtml");

        }

        public ActionResult CustomerColection()
        {

            return View("~/Views/Vehicle/Reports/CustomerColection.cshtml");

        }
        public ActionResult MonthlyCredits()
        {

            return View("~/Views/Vehicle/Reports/MonthlyCredits.cshtml");

        }
        public ActionResult OverDueAmounts()
        {

            return View("~/Views/Vehicle/Reports/OverDueAmounts.cshtml");

        }
        public ActionResult ActiveStocks()
        {

            return View("~/Views/Vehicle/Reports/ActiveStocks.cshtml");

        }
        public ActionResult TransferStocks()
        {

            return View("~/Views/Vehicle/Reports/TransferStocks.cshtml");

        }

        public ActionResult RepossedStocks()
        {

            return View("~/Views/Vehicle/Reports/RepossedStocks.cshtml");

        }
        public ActionResult DeliveryRports()
        {

            return View("~/Views/Vehicle/Reports/DeliveryRports.cshtml");

        }
        public ActionResult SoldStockAnalysis()
        {

            return View("~/Views/Vehicle/Reports/SoldStockAnalysis.cshtml");

        }
        public ActionResult SoldStockAnalysisByDates()
        {

            return View("~/Views/Vehicle/Reports/SoldStockAnalysisByDates.cshtml");

        }
        public ActionResult LogBookManagement()
        {

            return View("~/Views/Vehicle/LogBookManagement.cshtml");

        }
        public ActionResult Repossessions()
        {

            return View("~/Views/Vehicle/Reports/Repossessions.cshtml");

        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult GeneralLedger()
        {
            var list = new AccountRepository().GetLeafAccountsWithCodeName(SettingManager.SupplierHeadId);//.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
            ViewBag.Accounts = list;
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View("~/Views/Vehicle/Reports/GeneralLedger.cshtml");
        }

        public ActionResult VehiclePostDatedCheque()
        {

            ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            ViewBag.Settings = JsonConvert.SerializeObject(setting);

            return View("~/Views/Vehicle/VehiclePostDatedCheque.cshtml");

        }

        public ActionResult VehicleVoucher()
        {
            var type = Request.QueryString["type"] + "";
            var vouchertype = Numerics.GetInt(((VoucherType)Enum.Parse(typeof(VoucherType), type, true)));
            var settings = new List<SettingExtra>();
            settings.Add(new SettingExtra() { Key = "Supplier", Value = SettingManager.SupplierHeadId });
            settings.Add(new SettingExtra() { Key = "CashHeadId", Value = SettingManager.CashHeadId });
            settings.Add(new SettingExtra() { Key = "BankHeadId", Value = SettingManager.BankHeadId + "" });
            settings.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            ViewBag.Settings = JsonConvert.SerializeObject(settings);
            ViewBag.Vehicles = new VehicleRepository().GetVehicles();

            return View("~/Views/Vehicle/VehicleVoucher.cshtml");

        }
        public ActionResult ForexVoucher()
        {
            var settings = new List<SettingExtra>();
            settings.Add(new SettingExtra() { Key = "Supplier", Value = SettingManager.SupplierHeadId });
            settings.Add(new SettingExtra() { Key = "CashHeadId", Value = SettingManager.CashHeadId });
            settings.Add(new SettingExtra() { Key = "BankHeadId", Value = SettingManager.BankHeadId + "" });
            ViewBag.Settings = JsonConvert.SerializeObject(settings);
            ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View("~/Views/Vehicle/ForexVoucher.cshtml");

        }

    }
}
