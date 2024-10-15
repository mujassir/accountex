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
using Scriban;

namespace AccountEx.Web.Controllers.mvc
{
    //[Authorize]
    //[InitializeSimpleMembership]
    [Compress]
    public class TransactionController : BaseController
    {

        [Authorize]
        public ActionResult FormSetting()
        {
            ViewBag.Accounts = new AccountRepository().GetAccountTree();
            return View();
        }
        public void MapAction(int type, int voucherno)
        {
            var settingurl = SettingManager.Get("Other.Trans-" + Enum.GetName(typeof(VoucherType), type) + "-Url");
            var url = "";
            if (settingurl.StartsWith("~/"))
                url = settingurl + "&voucherno=" + voucherno;
            else if (settingurl.StartsWith("/"))
                url = "~" + settingurl + "&voucherno=" + voucherno;
            else if (!settingurl.StartsWith("~/"))
                url = "~/" + settingurl + "&voucherno=" + voucherno;
            if (!Response.IsRequestBeingRedirected)
                Response.Redirect(url, false);
        }
        [Authorize]
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Index()
        {
            var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            setting.Add(new FormSetting() { KeyName = "DiscountAccountId", Value = SettingManager.DiscountAccountId + "", VoucherType = Request["type"].ToLower() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult PurchaseWithRate()
        {
            var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult GstTransaction()
        {
            var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            setting.Add(new FormSetting() { KeyName = "GSTPercent", Value = SettingManager.Gst + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "GSTAccount", Value = SettingManager.GstHeadId + "", VoucherType = Request["type"].ToLower() });
            ViewBag.Voucher = Request["type"].ToLower();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());

            var itemAccountId = Numerics.GetInt(Request["ItemAccountId"]);
            var type = Request["type"];
            type = type.ToLower();
            var formtype = type == "sale" || type == "salereturn" ? (int)AccountDetailFormType.Customers : (int)AccountDetailFormType.Suppliers;

            ViewBag.AccountDetails = JsonConvert.SerializeObject(new
            {
                Customers = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == formtype)
                .ToList().Select(p => new
                {
                    Id = p.AccountId,
                    Name = p.Code + "-" + p.Name,
                    p.Code,
                    Title = p.Name,
                    p.Address
                }).ToList(),
                ItemAccounts = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == (int)AccountDetailFormType.Products).ToList().Select(p => new
                {
                    Id = p.AccountId,
                    Name = p.Code + "-" + p.Name,
                    p.Code,
                    Title = p.Name,
                }).ToList()

            });

            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult DiscountTransaction()
        {
            var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();



            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            return View();
        }
        public ActionResult Labours()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
                                                   //setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.LabourHeadAcHeadId + "" });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        public ActionResult Trans()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            var showDiscount = ((vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn) && SettingManager.IsSaleAllowDisocunt) || (((vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn) && SettingManager.IsPurchaseAllowDisocunt)) ? true : false;
            setting.Add(new SettingExtra() { Key = "ShowDiscount", Value = showDiscount });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            setting.Add(new SettingExtra() { Key = "CheckStockAvailability", Value = SettingManager.CheckStockAvailability });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult OstricSale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult KlassSale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetDiscountByProduct() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult LiquiMolySale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetDiscountByProduct() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult NoumtexSale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn || vouchertype == VoucherType.GstSale || vouchertype == VoucherType.GstSaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.GstSale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }

        public ActionResult NoumtexPurchase()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = (int)((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            //setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }


        public ActionResult NoumtexGstSale()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexSaleEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult TradeSale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "AdvanceTaxPercent", Value = SettingManager.AdvanceTax });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TradeSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
                //setting.Add(new SettingExtra() { Key = "Promotion", Value = new PromotionRepository().GetByDate(DateTime.Now.Date) });
                setting.Add(new SettingExtra() { Key = "Promotions", Value = new PromotionRepository().GetAllByFiscal() });
                setting.Add(new SettingExtra() { Key = "CustomerLesses", Value = new LessAssignmentToCustomerRepository().GetAllByFiscal() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TradePurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        public ActionResult POS()
        {

            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll() });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult JobOrderRequisition()
        {

            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<RequisitionEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult WeightSale()
        {


            ViewBag.Voucher = Request["type"].ToLower();
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransWeightSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransWeightPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = Numerics.BoolToLowerString(SettingManager.BarCodeEnabled) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult FlourSale()
        {
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "VehicleHeadId", Value = SettingManager.VehicleHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<FlourSaleEx>((byte)AccountDetailFormType.Products) });
            }

            // setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled ? true : false });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            setting.Add(new SettingExtra() { Key = "SoldRates", Value = new SaleRepository().GetLatestSaleRates() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {

            //        var voucnherNos = new GenericRepository<AccountEx.CodeFirst.Models.Transaction>().AsQueryable().
            //            Where(p => p.AccountId == 0 && p.TransactionType  == VoucherType.Sale)
            //            .Select(p => p.VoucherNumber).Distinct().ToList();
            //        var sales = new SaleRepository().AsQueryable().Where(p => p.TransactionType  == VoucherType.Sale && voucnherNos.Contains(p.VoucherNumber)).ToList();
            //        foreach (var sale in sales)
            //        {
            //            // AccountEx.BussinessLogic.TransactionManager.AddTransaction(sale, sale.CashSale);
            //        }

            //        scope.Complete();
            //    }
            //}
            return View();
        }
        public ActionResult IronSale()
        {

            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            ViewBag.Voucher = Request["type"].ToLower();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            //setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll<DiscountEx>() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
                if (vouchertype == VoucherType.Sale)
                    setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll<TransporterEx>() });
            }
            else if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });

            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        public ActionResult WheatPurchase()
        {

            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            // ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.WheatPurchaseSupplierHeadId });
            setting.Add(new SettingExtra() { Key = "WheatAccount", Value = SettingManager.Wheat });
            setting.Add(new SettingExtra() { Key = "WheatAccountHeadId", Value = SettingManager.WheatHeadId });
            setting.Add(new SettingExtra() { Key = "FoodDepartmentAccount", Value = SettingManager.FoodDepartmentAc });
            setting.Add(new SettingExtra() { Key = "FoodDepartmentHeadId", Value = SettingManager.FoodDepartmentHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll() });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            setting.Add(new SettingExtra() { Key = "Transporters", Value = new TransporterRepository().GetAll() });
            setting.Add(new SettingExtra() { Key = "SoldRates", Value = new WheatPurchaseRepository().GetLatestSoldRates() });
            setting.Add(new SettingExtra() { Key = "WheatPurcaseWeighBridgeAmount", Value = SettingManager.WheatPurcaseWeighBridgeAmount });
            setting.Add(new SettingExtra() { Key = "WheatPurcaseWeighBridgeAmount1", Value = SettingManager.WheatPurcaseWeighBridgeAmount1 });
            setting.Add(new SettingExtra() { Key = "WheatTradeAmount", Value = SettingManager.WheatTradeAmount });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult PharmacieSale()
        {

            //var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId + "" });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll() });
            if (vouchertype == VoucherType.Sale || vouchertype == VoucherType.SaleReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<PharmacySaleEx>((byte)AccountDetailFormType.Products) });
            }
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;

            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {
            //        var sales = new SaleRepository().AsQueryable().ToList();
            //        foreach (var sale in sales)
            //        {
            //            sale.FiscalId = SiteContext.Current.Fiscal.Id;
            //            // AccountEx.BussinessLogic.TransactionManager.AddTransaction(sale, sale.CashSale);
            //        }

            //        scope.Complete();
            //    }
            //}
            return View();
        }
        public ActionResult PharmacyPurchase()
        {
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId + "" });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll() });
            if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<PharmacySaleEx>((byte)AccountDetailFormType.Products) });
            }
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        public ActionResult PharmacyBonusPurchase()
        {
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true));
            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId + "" });
            setting.Add(new SettingExtra() { Key = "Discounts", Value = new CustomerDiscountRepository().GetAll() });
            if (vouchertype == VoucherType.Purchase || vouchertype == VoucherType.PurchaseReturn)
            {
                setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<PharmacySaleEx>((byte)AccountDetailFormType.Products) });
            }
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
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
        public ActionResult WorkInProgress()
        {
            var setting = new List<SettingExtra>();//.GetFormSettingByVoucherType(Request["type"]);
            //setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        public ActionResult WheatWorkInProcess()
        {

            //var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            //setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "WheatAccount", Value = SettingManager.Wheat });
            setting.Add(new SettingExtra() { Key = "WheatAccountHeadId", Value = SettingManager.WheatHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        public ActionResult OrderBooking()
        {
            var setting = new List<SettingExtra>();
            ViewBag.Currency = new GenericRepository<Currency>().GetAll();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            setting.Add(new SettingExtra() { Key = "IsMultipleLocationEnabled", Value = SettingManager.IsMultipleLocationEnabled });
            setting.Add(new SettingExtra() { Key = "RequiredPurchaseRequisition", Value = SettingManager.IsRequiredPurchaseRequisition });
            setting.Add(new SettingExtra() { Key = "RequiredSaleRequisition", Value = SettingManager.IsRequiredSaleRequisition });
            setting.Add(new SettingExtra() { Key = "ShowOnlyCustomerProducts", Value = SettingManager.ShowOnlyCustomerProducts });
            //if (SettingManager.BarCodeEnabled)
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<OrderDcEx>((byte)AccountDetailFormType.Products) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.Locations = new GenericRepository<UserLocation>().AsQueryable()
                .Where(x => x.UserId == (int)SiteContext.Current.User.Id)
                .Include(x => x.Location)
                .Select(x => x.Location)
                .ToList();
            return View();
        }
        public ActionResult Adjustment()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<AdjustmentEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult DeliveryChallan()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            //if (SettingManager.BarCodeEnabled)
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<UltratechDC>((byte)AccountDetailFormType.Products) });

            setting.Add(new SettingExtra() { Key = "IsMultipleLocationEnabled", Value = SettingManager.IsMultipleLocationEnabled });
            setting.Add(new SettingExtra() { Key = "RequiredPurchaseOrder", Value = SettingManager.IsRequiredPurchaseOrder });
            setting.Add(new SettingExtra() { Key = "RequiredSaleOrder", Value = SettingManager.IsRequiredSalesOrder });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.Locations = new GenericRepository<UserLocation>().AsQueryable()
                .Where(x => x.UserId == (int)SiteContext.Current.User.Id)
                .Include(x => x.Location)
                .Select(x => x.Location)
                .ToList();
            return View();
        }
        public ActionResult NTDeliveryChallan()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            setting.Add(new SettingExtra() { Key = "GST", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "VehicleHeadId", Value = SettingManager.VehicleHeadId });
            //if (SettingManager.BarCodeEnabled)
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexDCEx>((byte)AccountDetailFormType.Products) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }
        public ActionResult FoodSale()
        {

            var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            ViewBag.Voucher = Request["type"].ToLower();
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Suppliers", Value = SettingManager.SupplierHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "CashAccount", Value = SettingManager.CashAccountId + "", VoucherType = Request["type"].ToLower() });
            //setting.Add(new FormSetting() { KeyName = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        [OutputCache(CacheProfile = "IrisSale")]
        public ActionResult IrisSale()
        {


            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Services", Value = SettingManager.ServicesHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<IRISTransEx>((byte)AccountDetailFormType.Services) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = Numerics.BoolToLowerString(SettingManager.BarCodeEnabled) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {

            //        //var voucnherNos = new GenericRepository<AccountEx.CodeFirst.Models.Transaction>().AsQueryable().
            //        //    Where(p => p.AccountId == 0 && p.TransactionType  == VoucherType.Sale)
            //        //    .Select(p => p.VoucherNumber).Distinct().ToList();
            //        var sales = new SaleRepository().AsQueryable().AsNoTracking().
            //            Where(p => p.TransactionType  == VoucherType.Sale)
            //            .OrderBy(p=>p.Id).Skip(3*75).Take(75).ToList();
            //        foreach (var sale in sales)
            //        {
            //            IrisManager.FixData(sale);
            //        }

            //        scope.Complete();
            //    }
            //}
            return View();
        }
        public ActionResult JobCard()
        {


            ViewBag.Voucher = Request["type"].ToLower();
            var types = new List<byte>() { (byte)AccountDetailFormType.Services, (byte)AccountDetailFormType.Products };
            var setting = new List<SettingExtra>();

            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Employee", Value = SettingManager.EmployeeHeadId });
            setting.Add(new SettingExtra() { Key = "Services", Value = SettingManager.ServicesHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "GSTAccount", Value = SettingManager.GstHeadId });
            setting.Add(new SettingExtra() { Key = "Equipment", Value = SettingManager.EquipmentHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<JobCardEx>(types) });
            setting.Add(new SettingExtra() { Key = "EquipmentDetails", Value = new AccountDetailRepository().GetAll<EquipmentEx>((byte)AccountDetailFormType.Equipments) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult ServiceOrder()
        {



            var types = new List<byte>() { (byte)AccountDetailFormType.Services, (byte)AccountDetailFormType.Products };
            var setting = new List<SettingExtra>();

            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Sites", Value = SettingManager.SiteHeadId });
            setting.Add(new SettingExtra() { Key = "Employee", Value = SettingManager.EmployeeHeadId });
            setting.Add(new SettingExtra() { Key = "Services", Value = SettingManager.ServicesHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "GSTAccount", Value = SettingManager.GstHeadId });
            setting.Add(new SettingExtra() { Key = "Equipment", Value = SettingManager.EquipmentHeadId });
            setting.Add(new SettingExtra() { Key = "ExternalEquipment", Value = SettingManager.ExternalEquipmentHeadId });
            setting.Add(new SettingExtra() { Key = "InHouseEquipment", Value = SettingManager.InHouseEquipmentHeadId });
            setting.Add(new SettingExtra() { Key = "SiteEquipment", Value = SettingManager.SiteEquipmentHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<ServiceOrderEx>(types) });
            setting.Add(new SettingExtra() { Key = "EquipmentDetails", Value = new AccountDetailRepository().GetAll<EquipmentEx>((byte)AccountDetailFormType.Equipments) });
            setting.Add(new SettingExtra() { Key = "ProjectHeadId", Value = SettingManager.ProjectHeadId });
            setting.Add(new SettingExtra() { Key = "ExpensesHeadId", Value = SettingManager.ExpensesHeadId });


            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult UltratechSale()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<UltraTechTransSaleEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });


            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {

            //        var voucnherNos = new GenericRepository<AccountEx.CodeFirst.Models.Transaction>().AsQueryable().
            //            Where(p => p.AccountId == 0 && p.TransactionType  == VoucherType.Sale)
            //            .Select(p => p.VoucherNumber).Distinct().ToList();
            //        var sales = new SaleRepository().AsQueryable().Where(p => p.TransactionType  == VoucherType.Sale && voucnherNos.Contains(p.VoucherNumber)).ToList();
            //        foreach (var sale in sales)
            //        {
            //            UltraTechSaleManager.AddTransaction(sale, sale.CashSale);
            //        }

            //        scope.Complete();
            //    }
            //}


            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        public ActionResult UltratechPurchase()
        {


            ViewBag.Voucher = Request["type"].ToLower();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "GSTAccount", Value = SettingManager.GstHeadId });
            setting.Add(new SettingExtra() { Key = "CompanyId", Value = SiteContext.Current.User.CompanyId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<UltratechPurchaseEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {

            //        var voucnherNos = new GenericRepository<AccountEx.CodeFirst.Models.Transaction>().AsQueryable().
            //            Where(p => p.AccountId == 0 && p.TransactionType  == VoucherType.Purchase)
            //            .Select(p => p.VoucherNumber).Distinct().ToList();
            //        var sales = new SaleRepository().AsQueryable().Where(p => p.TransactionType  == VoucherType.Purchase && voucnherNos.Contains(p.VoucherNumber)).ToList();
            //        foreach (var sale in sales)
            //        {
            //            UltratechPurchaseManager.AddTransaction(sale);
            //        }

            //        scope.Complete();
            //    }
            //}
            return View();
        }

        public ActionResult VatTrans()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<UltraTechTransSaleEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });


            //if (Request["setdata"] == "data")
            //{
            //    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //    {
            //        var repo = new SaleRepository();
            //        var transTypes = new List<VoucherType>() { VoucherType.GstSale, VoucherType.GstPurchase, VoucherType.GstSaleReturn, VoucherType.GstPurchaseReturn };
            //        var sales = repo.AsQueryable().Where(p => transTypes.Contains(p.TransactionType)).ToList();
            //        foreach (var sale in sales)
            //        {
            //            VatTransManager.AddTransaction(sale, sale.CashSale, repo);
            //        }
            //        repo.SaveChanges();
            //        scope.Complete();
            //    }
            //}


            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        public ActionResult VatStockTransfer()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId + "" });
            setting.Add(new SettingExtra() { Key = "CashAccount", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "GSTPercent", Value = SettingManager.Gst });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<UltraTechTransSaleEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Voucher()
        {
            var rep = new AccountRepository();
            var account = rep.GetLeafAccount();
            ViewBag.AccountType = JsonConvert.SerializeObject(new AccountRepository().GetAll());
            ViewBag.Account = JsonConvert.SerializeObject(account);
            var setting = new FormSettingRepository().GetAll();
            var item = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var itemsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (itemsetting != null)
                item = Numerics.GetInt(itemsetting.Value);
            if (customersetting != null)
                Numerics.GetInt(customersetting.Value);
            ViewBag.Items = rep.GetLeafAccount(item);
            setting.Add(new FormSetting() { KeyName = "CashAccount", Value = SettingManager.CashAccountId + "", VoucherType = Request["type"].ToLower() });
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult PayablePayments()
        {
            var type = Request.QueryString["type"] + "";
            type = "CashPayments";
            var vouchertype = Numerics.GetInt(((VoucherType)Enum.Parse(typeof(VoucherType), type, true)));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "CashAccountId", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "BankHeadId", Value = SettingManager.BankHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BankAccountId", Value = SettingManager.BankAccountId });
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "CashHeadId", Value = SettingManager.CashHeadId });
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult BankVoucher()
        {
            var type = Request.QueryString["type"] + "";
            var vouchertype = (VoucherType)Numerics.GetInt(((VoucherType)Enum.Parse(typeof(VoucherType), type, true)));
            var settings = new List<SettingExtra>();
            settings.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            settings.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            settings.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            settings.Add(new SettingExtra() { Key = "IsBankReceiptAllowFinalization", Value = SettingManager.IsBankReceiptAllowFinalization });
            settings.Add(new SettingExtra() { Key = "IsBankPaymentAllowFinalization", Value = SettingManager.IsBankPaymentAllowFinalization });
            settings.Add(new SettingExtra() { Key = "TerritoryManagerHeadId", Value = SettingManager.TerritoryManagerHeadId });
            settings.Add(new SettingExtra() { Key = "AmountInWordType", Value = SettingManager.AmountInWordType });
            ViewBag.Settings = JsonConvert.SerializeObject(settings);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            if (vouchertype == VoucherType.BankReceipts && SettingManager.IsBankRecieptAllowPartner)
            {
                ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            }

            else if (vouchertype == VoucherType.BankPayments && SettingManager.IsBankPaymentAllowVehicle)
            {
                ViewBag.Vehicles = new VehicleRepository().GetVehicles();
            }
            ViewBag.CostCenters = new CostCenterRepository().GetNames();
            ViewBag.Cities = new CityRepository().GetNames();
            return View();
        }
        public ActionResult VoucherPosting()
        {
            return View();
        }
        public ActionResult VoucherTrans()
        {
            var type = Request.QueryString["type"] + "";
            var vouchertype = (VoucherType)Numerics.GetInt(((VoucherType)Enum.Parse(typeof(VoucherType), type, true)));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "CashAccountId", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "BankHeadId", Value = SettingManager.BankHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BankAccountId", Value = SettingManager.BankAccountId });
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Supplier", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "CashHeadId", Value = SettingManager.CashHeadId });
            setting.Add(new SettingExtra() { Key = "TerritoryManagerHeadId", Value = SettingManager.TerritoryManagerHeadId });
            setting.Add(new SettingExtra() { Key = "IsBankReceiptAllowFinalization", Value = SettingManager.IsBankReceiptAllowFinalization });
            setting.Add(new SettingExtra() { Key = "IsBankPaymentAllowFinalization", Value = SettingManager.IsBankPaymentAllowFinalization });
            setting.Add(new SettingExtra() { Key = "IsCashReceiptAllowFinalization", Value = SettingManager.IsCashReceiptAllowFinalization });
            setting.Add(new SettingExtra() { Key = "IsCashPaymentAllowFinalization", Value = SettingManager.IsCashPaymentAllowFinalization });
            setting.Add(new SettingExtra() { Key = "IsVehiclePayableAllowFinalization", Value = false });
            setting.Add(new SettingExtra() { Key = "AmountInWordType", Value = SettingManager.AmountInWordType });


            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();

            if (vouchertype == VoucherType.CashReceipts)
            {

                setting.Add(new SettingExtra() { Key = "AccountType", Value = SettingManager.CashRecieptAccountType });
                if (SettingManager.IsCashRecieptAllowVehicle)
                {
                    ViewBag.Vehicles = new VehicleRepository().GetVehicles();
                }
            }
            else if (vouchertype == VoucherType.CashPayments)
            {
                setting.Add(new SettingExtra() { Key = "AccountType", Value = SettingManager.CashPaymentAccountType });
                if (SettingManager.IsCashPaymentAllowVehicle)
                {
                    ViewBag.Vehicles = new VehicleRepository().GetVehicles();
                }

            }
            else if (vouchertype == VoucherType.BankReceipts)
            {
                setting.Add(new SettingExtra() { Key = "AccountType", Value = SettingManager.BankRecieptAccountType });
                if (SettingManager.IsBankRecieptAllowPartner)
                {
                    ViewBag.Vehicles = new VehicleRepository().GetVehicles();
                }
            }
            else if (vouchertype == VoucherType.BankPayments)
            {
                setting.Add(new SettingExtra() { Key = "AccountType", Value = SettingManager.BankPaymentAccountType });
                if (vouchertype == VoucherType.BankPayments && SettingManager.IsBankPaymentAllowVehicle)
                {
                    ViewBag.Vehicles = new VehicleRepository().GetVehicles();
                }
            }
            else if (vouchertype == VoucherType.VehiclePayable)
            {
                setting.Add(new SettingExtra() { Key = "AccountType", Value = 2 });

                ViewBag.Vehicles = new VehicleRepository().GetVehicles();

            }
            else if (vouchertype == VoucherType.AdvanceReceipts)
            {
                setting.Add(new SettingExtra() { Key = "AccountType", Value = 2 });
                ViewBag.Vehicles = new VehicleRepository().GetVehicles();

            }

            ViewBag.CostCenters = new CostCenterRepository().GetNames();
            ViewBag.Cities = new CityRepository().GetNames();
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult BLPayments()
        {
            var type = Request.QueryString["type"] + "";
            type = "CashPayments";
            var vouchertype = Numerics.GetInt(((VoucherType)Enum.Parse(typeof(VoucherType), type, true)));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "CashAccountId", Value = SettingManager.CashAccountId });
            setting.Add(new SettingExtra() { Key = "BankHeadId", Value = SettingManager.BankHeadId + "" });
            setting.Add(new SettingExtra() { Key = "BankAccountId", Value = SettingManager.BankAccountId });
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "CashHeadId", Value = SettingManager.CashHeadId });
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult SaleDiscounts()
        {
            var setting = new List<FormSetting>();
            setting.Add(new FormSetting() { KeyName = "CashAccount", Value = SettingManager.CashAccountId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Banks", Value = SettingManager.BankHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "CashAccount", Value = SettingManager.CashAccountId + "", VoucherType = Request["type"].ToLower() });

            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult JV()
        {
            var rep = new AccountRepository();
            var account = rep.GetLeafAccount();
            ViewBag.AccountType = JsonConvert.SerializeObject(new AccountRepository().GetAll());
            ViewBag.Account = JsonConvert.SerializeObject(account);
            var settings = new List<SettingExtra>();
            var item = 0;
            var customersetting = settings.FirstOrDefault(p => p.Key == "MasterAccountId");
            var itemsetting = settings.FirstOrDefault(p => p.Key == "ItemAccountId");
            if (itemsetting != null)
                item = Numerics.GetInt(itemsetting.Value);
            if (customersetting != null)
                Numerics.GetInt(customersetting.Value);
            ViewBag.Items = rep.GetLeafAccount(item);
            settings.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            settings.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            settings.Add(new SettingExtra() { Key = "IsJvAllowFinalization", Value = SettingManager.IsJvAllowFinalization });
            settings.Add(new SettingExtra() { Key = "AmountInWordType", Value = SettingManager.AmountInWordType });
            ViewBag.FormSettings = JsonConvert.SerializeObject(settings);
            ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            return View();
        }


        [OutputCache(CacheProfile = "Medium")]
        public ActionResult VoucherWithTax()
        {
            var rep = new AccountRepository();
            var account = rep.GetLeafAccount();
            ViewBag.AccountType = JsonConvert.SerializeObject(new AccountRepository().GetAll());
            ViewBag.Account = JsonConvert.SerializeObject(account);
            var setting = new FormSettingRepository().GetAll();
            var item = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var itemsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (itemsetting != null)
                item = Numerics.GetInt(itemsetting.Value);
            if (customersetting != null)
                Numerics.GetInt(customersetting.Value);
            setting.Add(new FormSetting() { KeyName = "CashAccount", Value = SettingManager.CashAccountId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Banks", Value = SettingManager.BankHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Customers", Value = SettingManager.CustomerHeadId + "", VoucherType = Request["type"].ToLower() });
            setting.Add(new FormSetting() { KeyName = "Products", Value = SettingManager.ProductHeadId + "", VoucherType = Request["type"].ToLower() });

            ViewBag.Items = rep.GetLeafAccount(item);
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult OpeningBalance()
        {

            //if (Request.Url != null && Request.Url.ToString().ToLower().Contains("klass"))
            //{
            //    var typeids = new AccountRepository().GetIdsByName(SettingManager.AccountType);
            //    var accounts = new AccountRepository().GetByLevel(3, typeids);
            //    accounts = accounts.Where(p => p.Name != SettingManager.Products && p.Name != "Expenses").ToList();
            //    ViewBag.Accounts = accounts;
            //}
            //else
            //{

            ViewBag.Voucher = VoucherType.OpeningBalance;
            var accounts = new AccountRepository().GetByLevel(3);
            accounts = accounts.Where(p => p.Name != SettingManager.Products).ToList();
            ViewBag.Accounts = accounts;
            //ViewBag.Projucts = new ProjectRepository().GetAll();
            //}


            return View();

        }
        public ActionResult RecoveryForm()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            ViewBag.FormSettings = JsonConvert.SerializeObject(setting);
            ViewBag.Voucher = "Recovery";
            return View();
        }
        public ActionResult StoreTransaction()
        {
            var rep = new AccountRepository();
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("StoreTransaction");
            var item = 0;
            var customer = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var itemsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (itemsetting != null)
                item = Numerics.GetInt(itemsetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            ViewBag.Customers = rep.GetLeafAccount(customer);
            ViewBag.Items = rep.GetLeafAccount(item);

            ViewBag.Voucher = "StoreTransaction";
            ViewBag.Markas = new MarkaRepository().GetIdNameMarka();
            ViewBag.Stores = new StoreRepository().GetIdNameMarka();
            var account = new AccountRepository().GetAll();
            ViewBag.AccountType = JsonConvert.SerializeObject(new AccountRepository().GetAll());
            ViewBag.Account = JsonConvert.SerializeObject(account);
            ViewBag.FormSettings = JsonConvert.SerializeObject(new FormSettingRepository().GetFormSettingByVoucherType("StoreTransaction"));
            ViewBag.Voucher = "StoreTransaction";
            var supplier = 0;

            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");

            if (suppliersetting != null)
                supplier = Numerics.GetInt(suppliersetting.Value);
            ViewBag.Suppliers = rep.GetLeafAccount(supplier);
            return View();
        }
        public string GetNextVoucherNumber()
        {
            string result;
            try
            {
                var po = new TransactionRepository().GetNextVoucherNumber(((VoucherType)Enum.Parse(typeof(VoucherType), Request["type"], true)));
                result = JsonResult(true, po + "");
            }
            catch (Exception ex)
            {

                result = JsonResult(false, ex.Message);
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
        public ActionResult BankReconciliation()
        {
            return View();
        }
        public ActionResult StockRequisition()
        {
            // var setting = new FormSettingRepository().GetAll();//.GetFormSettingByVoucherType(Request["type"]);
            // setting.ForEach(p => p.VoucherType = p.VoucherType.ToLower());
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "isMultipleLocationEnabled", Value = SettingManager.IsMultipleLocationEnabled });
            //setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<RequisitionEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            setting.Add(new SettingExtra() { Key = "ProductHeadId", Value = new AccountRepository().GetAccountTree(SettingManager.ProductHeadId).FirstOrDefault()?.Id ?? 0 });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.Locations = new GenericRepository<UserLocation>().AsQueryable()
                .Where(x => x.UserId == (int)SiteContext.Current.User.Id)
                .Include(x => x.Location)
                .Select(x => x.Location)
                .ToList();
            return View();
        }
        //
        // POST: /Account/Login
        public ActionResult GoodIssueNoteForProduction()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<RequisitionEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());

            //if (Request["setdata"] == "data")
            //{
            //    try
            //    {


            //        var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(2, 0, 0) };
            //        using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //        {



            //            var repo = new GINPRepository();

            //            var vouchertype = new List<VoucherType>();
            //            /////code for serviceorder addition


            //            vouchertype.Add(VoucherType.CustomerServiceOrder);
            //            vouchertype.Add(VoucherType.SiteServiceOrder);
            //            vouchertype.Add(VoucherType.RepairingServiceOrder);


            //            var data = repo.AsQueryable(true).Where(p => vouchertype.Contains(p.TransactionType)).ToList();

            //            foreach (var v in data)
            //            {
            //                DCManager.AddTransaction(v, repo);
            //                repo.SaveChanges();
            //            }

            //            scope.Complete();
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {
            //        ;

            //    }
            //}

            return View();
        }
        public ActionResult FinishedGoodReceivedNote()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId + "" });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<RequisitionEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }


        public ActionResult FiscalClosing()
        {

            // FiscalYearManager.CloseFiscalYear();
            var setting = new List<FormSetting>();//.GetFormSettingByVoucherType(Request["type"]);
            return View();
        }

        public ActionResult BLs()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<ShipperEx>((byte)AccountDetailFormType.Suppliers) });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Vehicles", Value = new VehicleRepository().GetVehicleAvailbaleForBL() });
            setting.Add(new SettingExtra() { Key = "BLExpensesHead", Value = SettingManager.BLExpenseHeadId + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.Statuses = new VehicleStatusRepository().GetNames();
            ViewBag.Shippers = new ShipeerRepository().GetNames();
            ViewBag.Consignees = new ConsigneeRepository().GetNames();
            ViewBag.Statuses = new VehicleStatusRepository().GetNames();
            ViewBag.Suppliers = new AccountRepository().GetLeafAccount(SettingManager.SupplierHeadId);

            return View();
        }

        public ActionResult InvoiceClearings()
        {
            //ViewBag.Customers = new AccountDetailRepository().GetNames(AccountDetailFormType.Customers);

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId + "" });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId + "" });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList());
            return View();
        }

        public ActionResult NTProduction()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<NoumtexSaleEx>((byte)AccountDetailFormType.Products) });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting.ToList()); ;
            return View();
        }
        public ActionResult VehicleSale()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Banks", Value = SettingManager.BankHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult BLStatuses()
        {
            ViewBag.Statuses = new VehicleStatusRepository().GetAll();
            ViewBag.Branches = new VehicleBranchRepository().GetNames();


            if (Request["setdata"] == "data")
            {
                try
                {


                    var txOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(2, 0, 0) };
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
                    {

                        var repo = new BLRepository();
                        var BLs = repo.GetAll();
                        var vehicleRepo = new VehicleRepository(repo);
                        var vehicleIds = BLs.SelectMany(p => p.BLItems).Select(p => p.VehicleId).ToList();
                        var vehicles = vehicleRepo.GetAll(p => vehicleIds.Contains(p.Id)).Where(p => p.BranchId > 0 && p.PurchasePrice > 0).ToList();

                        foreach (var v in vehicles)
                        {
                            BLStatusManager.AddTransaction(v, repo);
                            repo.SaveChanges();
                        }

                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }

            return View();
        }

 


        #region Helpers

        public JsonSerializerSettings GetJsonSetting()
        {
            var setting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "MM/dd/yyyy"
            };

            setting.Converters.Add(dateConverter);
            return setting;
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}
