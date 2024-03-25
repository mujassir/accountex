using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelectPdf;
using AccountEx.Repositories.Transactions;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.mvc
{
    public class ReportsController : BaseController
    {
        //
        // GET: /Reports/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult NTVehicleDetail()
        {
            return View();
        }
        public ActionResult IrisStock()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            return View();
        }
        public ActionResult SaleInvoice()
        {
            return View();
        }
        public ActionResult VoucherPrint()
        {
            return View();
        }
        public ActionResult XReports()
        {

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });

            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "ApplicationTitle", Value = SettingManager.ApplicationTitle });
            setting.Add(new SettingExtra() { Key = "AllowDynamicReportGraph", Value = SettingManager.AllowDynamicReportGraph });
            if (!string.IsNullOrWhiteSpace(Request["Name"]))
            {
                var Report = new ReportRepository().GetReportByName(Request["Name"]);
                setting.Add(new SettingExtra() { Key = "Report", Value = Report });
                ViewBag.ReportTitle = Report != null ? Report.Name : null;
            }
            else
                ViewBag.ReportTitle = null;
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Projects()
        {
            return View();
        }
        public ActionResult BLs()
        {
            return View();
        }
        public ActionResult LessDetail()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);

            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult OrderListing()
        {
            return View();
        }
        public ActionResult OrderByStatus()
        {
            return View();
        }
        public ActionResult Register()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Labour()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Employee", Value = SettingManager.EmployeeHeadId + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult VatRegister()
        {

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        public ActionResult UltratechRegister()
        {
            return View();
        }
        public ActionResult RegisterSummary()
        {
            return View();
        }
        public ActionResult ServiceCostRegister()
        {
            return View();
        }
        public ActionResult DayBook()
        {
            return View();
        }
        public ActionResult DailyActivity()
        {
            return View();
        }
        public ActionResult VehiclePostDatedCheque()
        {
            ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            ViewBag.Settings = JsonConvert.SerializeObject(setting);

            return View();

        }
        public ActionResult Invoice()
        {
            try
            {
                var voucherNumber = Numerics.GetInt(Request["voucher-number"]);
                var type = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));

                if (voucherNumber <= 0 || type <= 0) return View();
                var model = new InvoiceModel(new TransactionRepository().GetByVoucherNumber(type, voucherNumber));
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

        }
        public ActionResult GstInvoice()
        {
            try
            {
                var voucherNumber = Numerics.GetInt(Request["voucher-number"]);
                var type = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));

                if (voucherNumber <= 0 || type <= 0) return View();
                var model = new InvoiceModel(new SaleRepository().GetByVoucherNumber(voucherNumber, type));
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

        }

        public ActionResult KlassInvoice()
        {
            try
            {
                var voucherNumber = Numerics.GetInt(Request["voucher-number"]);
                var type = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));

                if (voucherNumber <= 0 || type <= 0) return View();
                var transactions = new TransactionRepository().GetByVoucherNumber(type, voucherNumber);
                var accountid = transactions.FirstOrDefault(p => p.EntryType == (byte)EntryType.MasterDetail).AccountId;
                var previoustransaction = new TransactionRepository().GetTransactions(accountid, 5);
                var model = new KlassInvoiceModels(transactions, previoustransaction);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

        }
        public ActionResult Invoice2()
        {
            try
            {
                var voucherNumber = Numerics.GetInt(Request["voucher-number"]);
                var type = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));

                if (voucherNumber > 0 && type > 0)
                {
                    var model = new InvoiceModel(new SaleRepository().GetByVoucherNumber(voucherNumber, type));
                    return View(model);
                }
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult TrialBalance()
        {
            return View();
        }
        public ActionResult RangeTrialBalance()
        {
            return View();
        }
        public ActionResult DetailTrialBalances()
        {
            return View();
        }
        public ActionResult Stock()
        {
            //var setting = new FormSettingRepository().GetFormSettingByVoucherType("Sale");
            //var itemsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            //if (itemsetting != null)
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            //else
            //    ViewBag.Accounts = new List<Account>();
            return View();
        }
        public ActionResult StockByQuantity()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            return View();
        }
        public ActionResult StockByWeight()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            return View();
        }
        public ActionResult DayBookReport()
        {
            return View();
        }

        public ActionResult PharmacyStockWithNotes()
        {

            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            //ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            return View();
        }
        public ActionResult PharmacyStock()
        {
            //var setting = new FormSettingRepository().GetFormSettingByVoucherType("Sale");
            //var itemsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            //if (itemsetting != null)
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Accounts = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId);
            //else
            //    ViewBag.Accounts = new List<Account>();
            return View();
        }


        public ActionResult ProductLedger()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);

            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult ProductTrans()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);

            setting.Add(new FormSetting() { KeyName = "ProductHeadId", Value = SettingManager.ProductHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult ProductTransSummary()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            setting.Add(new FormSetting() { KeyName = "ProductHeadId", Value = SettingManager.ProductHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult ProductionDetail()
        {
            return View();
        }

        public ActionResult PartyTrans()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);

            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult PartyTrans1()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);

            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new FormSetting() { KeyName = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult ProfitLoss()
        {

            // FiscalYearManager.CloseFiscalYear();


            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);


            return View();
        }
        public ActionResult ProfitLossFormat1()
        {

            // FiscalYearManager.CloseFiscalYear();
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            return View();
        }

        public ActionResult ProfitLossAM()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            return View();
        }
        public ActionResult SalePurchaseSummary()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            return View();
        }
        public ActionResult YearlyComparison()
        {
            //var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            return View();
        }
        public ActionResult CustomerAging()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }

        public ActionResult BalanceSheet()
        {

            return View();
        }
        public ActionResult PeriodicBalances()
        {
            return View();
        }

        public ActionResult SaleReportByArea()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }
        public ActionResult DailyProfitLoss()
        {
            return View();
        }

        public ActionResult UsmanBrosCustomers()
        {
            return View();
        }

        public ActionResult SaleReportByAreaDateRange()
        {
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "" });
            var data = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.FormSetting = data;
            return View();
        }

        public ActionResult StaticLedger()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult SalarySheet()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult CustomerBalances()
        {
            ViewBag.Cities = new GenericRepository<City>().GetNames();
            ViewBag.Groups = new GenericRepository<CustomerGroup>().GetNames();
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult CustomerRecovery()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult SupplierBalances()
        {
            ViewBag.Groups = new GenericRepository<SupplierGroup>().GetNames();
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult AccountBalances()
        {
            ViewBag.Accounts = new AccountRepository().GetByLevel(2);
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult AccountBalances1()
        {
            ViewBag.Accounts = new AccountRepository().GetByLevel(2);
            return View();
        }
        [HttpGet]
        public ActionResult SaleDetail()
        {
            return View();
        }
        public ActionResult ReportSummary()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Assets()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Banks()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Customers()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult CustomersWithGroup()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Employees()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Products()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Medicines()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Suppliers()
        {
            return View();
        }

        [ActionName("SaleDetail")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaleDetail_Post()
        {
            //var voucherNumber = Numerics.GetInt((Request["VoucherNumber"]));
            //var sale = new SaleRepository().GetByVoucherNumber(voucherNumber);
            return View();
        }
        [HttpGet]
        public ActionResult SaleSummary()
        {
            return View();
        }
        [ActionName("SaleSummary")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaleSummary_Post()
        {
            //var VoucherNumber = Numerics.GetInt(Request["VoucherNumber"]);
            //var sale = new SaleRepository().GetByVoucherNumber(VoucherNumber);
            return View("");
        }


        [OutputCache(CacheProfile = "Long")]
        public ActionResult VehicleLedger()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            var type = (Request.QueryString["type"] + "").ToLower();
            list = ViewBag.Vehicles = new vw_VehicleSalesRepository().GetVehiclesWithCustomer();
            model.Accounts = list;
            return View(model);
        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult VehicleProfile()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            var type = (Request.QueryString["type"] + "").ToLower();
            list = ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            model.Accounts = list;
            return View(model);
        }

        [OutputCache(CacheProfile = "Long")]
        public ActionResult GeneralLedger()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            var type = (Request.QueryString["type"] + "").ToLower();
            var accountId = 0;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["accountId"])) accountId = Numerics.GetInt(Request.QueryString["accountId"]);
            if (type == "cash" && accountId == 0) accountId = SettingManager.CashAccountId;
            if (accountId > 0)
            {
                var account = new AccountRepository().GetById(accountId);
                list.Add(new IdName() { Id = account.Id, Name = account.AccountCode + "-" + account.Name });
            }
            else if (!string.IsNullOrWhiteSpace(type))
            {
                var accountDetailFormType = (AccountDetailFormType)Enum.Parse(typeof(AccountDetailFormType), type, true);
                list = new AccountDetailRepository().GetNames(accountDetailFormType);
            }
            else
                list = new AccountRepository().GetLeafAccountsWithCodeName();//.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
            model.Accounts = list;
            return View(model);
        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult GetGeneralLedgerWithMultiAccouts()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            list = new AccountRepository().GetLeafAccountsWithCodeName();//.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
            model.Accounts = list;
            return View(model);
        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult DetailedGeneralLedger()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            var type = (Request.QueryString["type"] + "").ToLower();
            var accountId = 0;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["accountId"])) accountId = Numerics.GetInt(Request.QueryString["accountId"]);
            if (type == "cash" && accountId == 0) accountId = SettingManager.CashAccountId;
            if (accountId > 0)
            {
                var account = new AccountRepository().GetById(accountId);
                list.Add(new IdName() { Id = account.Id, Name = account.AccountCode + "-" + account.Name });
            }
            else if (!string.IsNullOrWhiteSpace(type))
            {
                var accountDetailFormType = (AccountDetailFormType)Enum.Parse(typeof(AccountDetailFormType), type, true);
                list = new AccountDetailRepository().GetNames(accountDetailFormType);
            }
            else
                list = new AccountRepository().GetLeafAccountsWithCodeName();//.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
            model.Accounts = list;
            return View(model);
        }
        [OutputCache(CacheProfile = "Long")]
        public ActionResult NTDetailedGeneralLedger()
        {
            var model = new GeneralLedgerModel();
            var list = new List<IdName>();
            var type = (Request.QueryString["type"] + "").ToLower();
            var accountId = 0;
            if (!string.IsNullOrWhiteSpace(Request.QueryString["accountId"])) accountId = Numerics.GetInt(Request.QueryString["accountId"]);
            if (type == "cash" && accountId == 0) accountId = SettingManager.CashAccountId;
            if (accountId > 0)
            {
                var account = new AccountRepository().GetById(accountId);
                list.Add(new IdName() { Id = account.Id, Name = account.AccountCode + "-" + account.Name });
            }
            else if (!string.IsNullOrWhiteSpace(type))
            {
                var accountDetailFormType = (AccountDetailFormType)Enum.Parse(typeof(AccountDetailFormType), type, true);
                list = new AccountDetailRepository().GetNames(accountDetailFormType);
            }
            else
                list = new AccountRepository().GetLeafAccountsWithCodeName();//.Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
            model.Accounts = list;
            return View(model);
        }
        public ActionResult VoucherList()
        {
            var model = new GeneralLedgerModel();
            model.Accounts = new AccountRepository().GetLeafAccountsWithCodeName();
            return View(model);
        }
        [HttpPost]
        public ActionResult GeneralLedger(GeneralLedgerModel model)
        {
            var repo = new TransactionRepository();
            model.Accounts = new AccountRepository().GetLeafAccountsWithCodeName();
            model.OpeningBalance = repo.GetOpeningBalance(model.AccountId, model.FromDate);
            model.Transactions = repo.GetTransactions(model.AccountId, model.FromDate, model.ToDate);
            return View(model);
        }
        public ActionResult DetailedLedger()
        {
            var model = new DetailedLedgerModel { Accounts = new AccountRepository().GetLeafAccountsWithCodeName() };
            return View(model);
        }
        [HttpPost]
        public ActionResult DetailedLedger(DetailedLedgerModel model)
        {
            var repo = new TransactionRepository();
            model.Accounts = new AccountRepository().GetLeafAccountsWithCodeName();
            model.OpeningBalance = repo.GetOpeningBalance(model.AccountId, model.FromDate);
            model.Transactions = new List<TransactionExtra>(); //repo.GetDetailedTransactions(model.AccountId, model.FromDate, model.ToDate);
            return View(model);
        }
        public ActionResult SaleLedger()
        {
            var model = new SaleLedgerModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult SaleLedger(SaleLedgerModel model)
        {
            model.Sales = new SaleRepository().AsQueryable().Where(p => p.Date >= model.FromDate && p.Date <= model.ToDate).ToList();
            return View(model);
        }
        public ActionResult NTDataExport()
        {
            return View();
        }

        public ActionResult VehicleInstallmentDetail()
        {
            return View();
        }

        //public string LoadGenericReportList()
        //{
        //    string result;
        //    try
        //    {
        //        var record = new ReportRepository().GetAll().OrderBy(p => p.Order);
        //        result = JsonResult(true, JsonConvert.SerializeObject(record, GetJsonSetting()));
        //    }
        //    catch (Exception ex)
        //    {
        //        result = JsonResult(false, ex.Message);
        //    }
        //    return result;

        //}
        //public string LoadReportParameters()
        //{
        //    string result;
        //    try
        //    {
        //        var reportId = Numerics.GetInt(Request["reportid"]);
        //        var record = new ReportParameterRepository().GetByReportId(reportId);
        //        foreach (var item in record)
        //        {
        //            if (item.Type == "select")
        //            {
        //                //Set up and invoke the reporting method
        //                var reportManager = Type.GetType(item.SelectionClass);
        //                var classInstance = Activator.CreateInstance(reportManager, null);
        //                if (reportManager != null)
        //                {
        //                    var reportMethod = reportManager.GetMethod(item.SelectionMethod);
        //                    var data = reportMethod.Invoke(classInstance, null);
        //                    item.SelectionMethod = JsonConvert.SerializeObject(data);






        //                }
        //            }
        //            item.SelectionClass = JsonConvert.SerializeObject(new ReportColumnRepository().GetByReportId(reportId));

        //        }
        //        result = JsonResult(true, JsonConvert.SerializeObject(record, GetJsonSetting()));
        //    }
        //    catch (Exception ex)
        //    {

        //        result = JsonResult(false, ex.Message);
        //    }
        //    return result;

        //}
        public string GetReportData()
        {

            var data = JsonConvert.DeserializeObject<List<ParameterExtra>>(Request["record"]);
            var id = Numerics.GetInt(Request["reportid"]);
            var result = "";

            var reportPageSize = 20;
            // page = (page ?? 1) - 1;

            var report = new ReportRepository().GetReportById(id);
            ////check report permissions
            //string[] roleNames = report.Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToArray();
            //if (SiteContext.Current.User.CustomerRoles.Any(cr => roleNames.Contains(cr.Name)))
            //{
            //set up the parameter list
            var reportParameters = new List<ReportParameterExtra>();
            //reportParameters.Add(new ReportParameter { ParameterName = "programid", ParameterValue = SiteContext.Current.CurrentProgramID.Value });
            //reportParameters.Add(new ReportParameter { ParameterName = "customerid", ParameterValue = SiteContext.Current.User.CustomerId });
            foreach (var key in data)
            {
                reportParameters.Add(new ReportParameterExtra { ParameterName = key.Key, ParameterValue = key.Value });
            }
            var reportLines = ReportManager.LoadReport(report, reportParameters).ToList();
            //int totalRecords = reportLines.Count;
            //reportLines = reportLines.GetRange(page.Value * reportPageSize, Math.Min(reportPageSize, (totalRecords - reportPageSize * page.Value)));
            result = JsonResult(true, JsonConvert.SerializeObject(reportLines, GetJsonSetting()));
            //}

            return result;

        }

        public JsonSerializerSettings GetJsonSetting()
        {
            var setting = new JsonSerializerSettings();
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "MM/dd/yyyy"
            };

            setting.Converters.Add(dateConverter);
            return setting;
        }
        public string JsonResult(bool success, string data)
        {
            return new JavaScriptSerializer().Serialize(new
            {
                Result = success,
                Data = data,
            });
        }

    }
}
