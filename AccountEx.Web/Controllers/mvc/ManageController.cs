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
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Web.Controllers.mvc
{
    [Compress]
    public class ManageController : BaseController
    {
        //
        // GET: /Project/

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Employees()
        {
            var setting = new List<SettingExtra>();
            Session["rootpath"] = Server.MapPath("../");
            ViewBag.Department = new GenericRepository<Department>().GetNames();
            ViewBag.Designation = new GenericRepository<Designation>().GetNames();
            setting.Add(new SettingExtra() { Key = "Banks", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Banks) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);

            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Customers()
        {
            ViewBag.VendorCodeCustomers = JsonConvert.SerializeObject(new AccountDetailRepository().GetByOwnVendorCodes());
            ViewBag.HeadAccount = new AccountRepository().GetByName(AccountManager.CustomerAccountTitle);
            ViewBag.Salesman = new AccountRepository().GetLeafAccounts(SettingManager.SalemanHeadId);
            ViewBag.CustomerGroups = new GenericRepository<CustomerGroup>().GetAll();
            ViewBag.Cities = new GenericRepository<City>().GetNames();
            return View();
        }

        public ActionResult Services()
        {
            ViewBag.HeadAccount = new AccountRepository().GetByName(AccountManager.CustomerAccountTitle);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }

        //public ActionResult Employeeincome()
        //{
        //    ViewBag.Department = new GenericRepository<Department>().GetNames();
        //    ViewBag.Designation = new GenericRepository<Designation>().GetNames();

        //    var setting = new List<SettingExtra>();
        //    //setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
        //    //setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
        //    //setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });

        //    setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
        //    setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
        //    ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
        //    return View();
        //}

        public ActionResult Transporters()
        {
            //ViewBag.HeadAccount = new AccountRepository().GetByName(AccountManager.CustomerAccountTitle);
            return View();
        }

        public ActionResult Currencies()
        {
            return View();
        }

        public ActionResult CurrencyRates()
        {
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View();
        }
        public ActionResult SupplierCurrencies()
        {
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            ViewBag.Suppliers = new AccountDetailRepository().GetNames(AccountDetailFormType.Suppliers);
            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Labour()
        {
            ViewBag.Salesman = new AccountRepository().GetLeafAccounts(SettingManager.SalemanHeadId);
            ViewBag.CustomerGroups = new GenericRepository<CustomerGroup>().GetAll();
            ViewBag.Cities = new GenericRepository<City>().GetNames();
            return View();
        }

        public ActionResult Models()
        {
            return View();
        }
        public ActionResult Vehicles()
        {
            ViewBag.Colors = new ColorRepository().GetNames();
            ViewBag.Manufactures = new ManufactureRepository().GetNames();
            ViewBag.Fuel = new FuelRepository().GetNames();
            ViewBag.CarType = new CarTypeRepository().GetNames();
            ViewBag.Model = new ModelRepository().GetNames();
            ViewBag.Brands = new BrandsRepository().GetNames();
            ViewBag.EnginePower = new EnginePowerRepository().GetNames().OrderBy(p => p.Name).ToList();
            ViewBag.ClearingCompany = new ClearingCompanyRepository().GetNames();
            ViewBag.VehicleBranches = new VehicleBranchRepository().GetNames();
            ViewBag.Year = new YearRepository().GetNames();
            ViewBag.Month = new MonthRepository().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            setting.Add(new SettingExtra() { Key = "Suppliers", Value = SettingManager.SupplierHeadId });
            setting.Add(new SettingExtra() { Key = "ForexSuppliers", Value = new AccountDetailRepository().GetForexSuppliers() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult VehicleBranches()
        {

            return View();
        }

        public ActionResult CompanyPartners()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Suppliers()
        {
            ViewBag.SupplierGroups = new GenericRepository<SupplierGroup>().GetNames();
            ViewBag.Salesman = new AccountRepository().GetLeafAccounts(SettingManager.SalemanHeadId);
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Banks()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult City()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Templates()
        {
            ViewBag.Tags = new TemplateTagRepository().GetNames();
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult ProductGroups()
        {
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult CustomerGroups()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Products()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.UnitTypes = new GenericRepository<UnitType>().GetNames();
            ViewBag.Brands = new BrandsRepository().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Equipments()
        {
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.UnitTypes = new GenericRepository<UnitType>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Assets()
        {
            ViewBag.AssetTypes = new GenericRepository<AssetType>().GetAll();
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Salesman()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Tenants()
        {
            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult OrderTakers()
        {
            return View();
        }
        public ActionResult TerritoryManagers()
        {
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult MedicineItems()
        {
            ViewBag.Generics = new GenericRepository<Generic>().GetNames();
            ViewBag.Manufactures = new ManufactureRepository().GetNames();
            ViewBag.ProductGroups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.PackagingTypes = new GenericRepository<PackagingType>().GetNames();
            return View();
        }

        public ActionResult SupplierProducts()
        {
            ViewBag.Suppliers = new AccountDetailRepository().GetNames(AccountDetailFormType.Suppliers);
            ViewBag.Generics = new GenericRepository<Generic>().GetNames();
            ViewBag.Manufacturers = new ManufactureRepository().GetNames();
            return View();
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Stockopeningbalance()
        {
            return View();
        }
        public ActionResult Settings()
        {
            Session["rootpath"] = Server.MapPath("../");
            return View();
        }
        public ActionResult salarysetup()
        {
            return View();
        }
        public ActionResult TradeSuppliers()
        {
            return View();
        }
        public ActionResult TradeCustomers()
        {
            ViewBag.Country = new CountryRepository().GetNames();
            return View();
        }
        public ActionResult PackagesType()
        {

            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Discounts()
        {
            ViewBag.Voucher = VoucherType.OpeningBalance;
            var rep = new AccountRepository();
            var accounts = rep.GetLeafAccounts(Numerics.GetInt(SettingManager.CustomerHeadId));
            ViewBag.Customers = accounts;
            return View();
        }
        [OutputCache(CacheProfile = "Medium")]
        public ActionResult ProductMapping()
        {
            ViewBag.Voucher = VoucherType.OpeningBalance;
            var rep = new AccountRepository();
            var accounts = rep.GetLeafAccounts(Numerics.GetInt(SettingManager.CustomerHeadId));
            ViewBag.Customers = accounts;
            return View();
        }

        public ActionResult productwisediscounts()
        {

            return View();
        }
        public ActionResult MenuManagement()
        {
            ViewBag.Menues = new MenuItemRepository().GetByCompanyId((int)SiteContext.Current.User.CompanyId);
            return View();
        }

        public ActionResult productversions()
        {
            return View();
        }
        public ActionResult departments()
        {
            return View();
        }
        public ActionResult Brands()
        {
            return View();
        }
        public ActionResult Manufacturers()
        {
            return View();
        }
        public ActionResult Generics()
        {
            return View();
        }
        public ActionResult Designations()
        {
            return View();
        }
        public ActionResult UnitTypes()
        {
            return View();
        }
        public ActionResult Promotions()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "BarCodeEnabled", Value = SettingManager.BarCodeEnabled });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.PromotionItemGroups = new ItemGroupRepository().GetPromotionItemGroups();
            return View();
        }
        public ActionResult ItemGroups()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult CustomerIncentiveGroups()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Customers", Value = SettingManager.CustomerHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult promotionassignment()
        {
            ViewBag.PromotionCustomerGroups = new ItemGroupRepository().GetPromotionCustomerGroups();
            ViewBag.Promotions = new PromotionRepository().GetAll();
            return View();
        }
        public ActionResult LessAssignmentToCustomers()
        {
            ViewBag.LessCustomerGroups = new ItemGroupRepository().GetLessCustomerGroups();
            ViewBag.LessItemGroups = new ItemGroupRepository().GetLessItemGroups();
            return View();
        }
        public ActionResult ShopSetup()
        {
            ViewBag.Blocks = new GenericRepository<Block>().GetNames();
            return View();
        }
        public ActionResult Blocks()
        {
            return View();
        }
        public ActionResult RentAgreement()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Transfers()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult ElectricityUnits()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "ElectriciyUnitCharges", Value = SettingManager.ElectricityPerUnitCost });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            //ViewBag.CompanyPartners = new CompanyPartnerRepository().GetNames();
            //ViewBag.Blocks = new BlockRepository().GetNames();
            return View();
        }
        public ActionResult Challans()
        {
            var setting = new List<SettingExtra>();
            //setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult MiscChargesChallans()
        {
            var setting = new List<SettingExtra>();
            //setting.Add(new SettingExtra() { Key = "Tenants", Value = SettingManager.TenantHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TenantEx>((byte)AccountDetailFormType.Tenant) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult RentBill()
        {
            //ViewBag.Tenants = new AccountDetailRepository().GetNames(AccountDetailFormType.Tenant);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "RentDueDate", Value = SettingManager.RentDueDate });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult VehicleStatus()
        {
            return View();
        }
        public ActionResult VehicleSendRequest()
        {
            ViewBag.VehicleBranches = new VehicleBranchRepository().GetNames();
            ViewBag.Vehicles = JsonConvert.SerializeObject(new VehicleRepository().GetAll());
            return View();
        }
        public ActionResult Shippers()
        {
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View();
        }
        public ActionResult Ships()
        {
            ViewBag.Shippers = new ShipeerRepository().GetNames();
            return View();
        }
        public ActionResult ClasificationOfAsserts()
        {
            ViewBag.ClasificationOfAsserts = new ShipeerRepository().GetNames();
            return View();
        }
        public ActionResult Expenses()
        {

            return View();
        }
        public ActionResult BarcodePrinting()
        {

            return View();
        }
        public ActionResult Documents()
        {
            return View();
        }
    }
}
