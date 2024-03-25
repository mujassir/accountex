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
using AccountEx.CodeFirst.Models.Config;

namespace AccountEx.Web.Controllers.mvc
{
    public class CRMController : BaseController
    {
        //
        // GET: /CRM/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FollowUp()
        {
            return View();
        }

        public ActionResult Leads()
        {
            //ViewBag.Salesman = new AccountDetailRepository().GetNames(AccountDetailFormType.Salesman);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.Industry = new IndustryRepository().GetNames();
            ViewBag.Sector = new WorkingSectorRepository().GetNames();
            return View();
        }
        public ActionResult LeadActivities()
        {

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Salesman", Value = SettingManager.SalemanHeadId });
            setting.Add(new SettingExtra() { Key = "Products", Value = SettingManager.ProductHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<TransSaleEx>((byte)AccountDetailFormType.Products) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            //ViewBag.LeadOwner = new LeadActivityRepository().GetAll();
            return View();
        }
        public ActionResult Divisions()
        {
            ViewBag.Categories = new GenericRepository<ProductCategory>().GetNames();
            return View();
        }
        public ActionResult Domains()
        {
            return View();
        }
        public ActionResult UserTypes()
        {
            return View();
        }
        public ActionResult Regions()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }
        public ActionResult ProductSecondaryCategories()
        {
            return View();
        }

        public ActionResult productsubgroups()
        {
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            return View();
        }
        public ActionResult ProductStatuses()
        {
            return View();
        }
        public ActionResult UOMS()
        {

            return View();
        }

        public ActionResult Provinces()
        {

            return View();
        }
        public ActionResult ClusterTypes()
        {

            return View();
        }
        public ActionResult ProductLostReasons()
        {
            return View();
        }
        public ActionResult ContactBook()
        {
            ViewBag.Groups = new GenericRepository<ContactGroup>().GetNames();
            return View();
        }
        public ActionResult ContactGroup()
        {
            return View();
        }
        public ActionResult Users()
        {
            ViewBag.Roles = new GenericRepository<Role>().GetNames();
            ViewBag.UserTypes = new GenericRepository<UserType>().GetNames();
            ViewBag.Domains = new GenericRepository<Domain>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.RSM = new UserRepository().LoadCRMUserByType(CRMUserType.RSM);
            return View();
        }
        public ActionResult Products()
        {
            ViewBag.Categories = new GenericRepository<ProductCategory>().GetNames();
            ViewBag.SecCategories = new GenericRepository<ProductSecCategory>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Vendors = new GenericRepository<CRMVendor>().GetNames();
            return View();
        }
        public ActionResult Customers()
        {
            var setting = new List<SettingExtra>();
            var userRepo = new UserRepository();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();

            var isSalePerson = userRepo.CheckUserType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
            if (isSalePerson)
            {
                ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
            }
            else
            {
                ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            }



            setting.Add(new SettingExtra() { Key = "RegionId", Value = SiteContext.Current.RegionId });
            setting.Add(new SettingExtra() { Key = "UserTypeId", Value = SiteContext.Current.UserTypeId });
            ViewBag.UOMS = new GenericRepository<UOM>().GetNames();
            ViewBag.Cities = new GenericRepository<City>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            ViewBag.ClusterTypes = new GenericRepository<ClusterType>().GetNames();
            ViewBag.Provinces = new GenericRepository<Province>().GetNames();
            ViewBag.Categories = new GenericRepository<ProductCategory>().GetNames();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Vendors()
        {
            ViewBag.UOMS = new GenericRepository<UOM>().GetNames();
            return View();
        }

        public ActionResult PMC()
        {
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames("Unit");
            ViewBag.Fiscals = new FiscalRepository().GetNames();
            var setting = new List<SettingExtra>();
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            setting.Add(new SettingExtra() { Key = "IsSalePerson", Value = SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive ? true : false });
            setting.Add(new SettingExtra() { Key = "UserId", Value = SiteContext.Current.User.Id });
            if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
            {
                var customerCategories = new CRMCustomerRepository().GetCategoriesWithCustomer(SiteContext.Current.User.Id);
                setting.Add(new SettingExtra() { Key = "CustomerCategories", Value = customerCategories });
            }
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Invoice()
        {
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Statuses = new GenericRepository<ProductStatus>().GetNames("Name", "ProductStatuses");
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetNames(SiteContext.Current.UserTypeId);
            ViewBag.SaleTypes = new GenericRepository<SaleType>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames("Unit");
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            return View();
        }
        public ActionResult IGRN()
        {
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Statuses = new GenericRepository<ProductStatus>().GetNames("Name", "ProductStatuses");
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetNames(SiteContext.Current.UserTypeId);
            ViewBag.SaleTypes = new GenericRepository<SaleType>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames("Unit");
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            return View();
        }
        public ActionResult ImportRequisition()
        {
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Suppliers = new CRMVendorRepository().GetIdNameByType(CRMVendorType.Import);
            ViewBag.SaleTypes = new GenericRepository<SaleType>().GetNames();
            ViewBag.POLS = new GenericRepository<PortofLoading>().GetNames();
            ViewBag.PODS = new GenericRepository<PortOfDischarge>().GetNames();
            ViewBag.Destinations = new GenericRepository<Destination>().GetNames();
            ViewBag.Vessels = new GenericRepository<Vessel>().GetNames();
            ViewBag.ContractTypes = new GenericRepository<ContractType>().GetNames();
            ViewBag.DeliveryTerms = new GenericRepository<DeliveryTerm>().GetNames();
            ViewBag.UOMS = new GenericRepository<UOM>().GetNames().Where(p => p.Name.ToLower() == "kgs").ToList();
            ViewBag.Currencies = new CurrencyRepository().GetNames("Unit");
            return View();
        }
        public ActionResult Projects()
        {
            var userRepo = new UserRepository();
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Statuses = new ProductStatusRepository().GetIdName();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetProductsIdName(SiteContext.Current.UserTypeId);
            ViewBag.LostReasons = new GenericRepository<ProductLostReason>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            ViewBag.Fiscals = new FiscalRepository().GetNames();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "IsSalePerson", Value = SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive ? true : false });
            setting.Add(new SettingExtra() { Key = "UserId", Value = SiteContext.Current.User.Id });
            if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
            {
                var customerCategories = new CRMCustomerRepository().GetCategoriesWithCustomer(SiteContext.Current.User.Id);
                setting.Add(new SettingExtra() { Key = "CustomerCategories", Value = customerCategories });
            }
            var pmcItemId = Numerics.GetInt(Request["pmcId"]);
            var pmcId = 0;
            if (pmcItemId > 0)
            {
                pmcId = new CRMProjectRepository().GetIdbyPMCItem(pmcItemId);
                if (pmcId == 0)
                    setting.Add(new SettingExtra() { Key = "PMCItem", Value = new PMCItemRepository().GetByIdFromView(pmcItemId) });

            }

            setting.Add(new SettingExtra() { Key = "VoucherNo", Value = pmcId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult EventModes()
        {
            return View();
        }
        public ActionResult EventStatuses()
        {
            return View();
        }
        public ActionResult SaleTypes()
        {
            return View();
        }
        public ActionResult calendarevents()
        {
            var userRepo = new UserRepository();
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.EventModes = new GenericRepository<EventMode>().GetNames();
            ViewBag.EventStatuses = new GenericRepository<EventStatus>().GetNames("Name", "EventStatuses");
            ViewBag.Statuses = new GenericRepository<ProductStatus>().GetNames("Name", "ProductStatuses");
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetProductsIdName(SiteContext.Current.UserTypeId);
            ViewBag.LostReasons = new GenericRepository<ProductLostReason>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            ViewBag.Fiscals = new FiscalRepository().GetNames();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = new CRMProductRepository().GetOwnProductsIdName() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult complaints()
        {
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.CaseTypes = new GenericRepository<CRMCaseType>().GetNames();
            ViewBag.Priorities = new GenericRepository<Priority>().GetNames("Name", "Priorities");
            ViewBag.TestTypes = new GenericRepository<ComplaintTest>().GetNames();
            ViewBag.Labs = new GenericRepository<ComplaintLab>().GetNames();
            ViewBag.Users = new UserRepository().LoadCRMUserByType(new List<CRMUserType> { CRMUserType.SalesExecutive, CRMUserType.LabUser });
            var userRepo = new UserRepository();
            var userType = userRepo.GetUserType(SiteContext.Current.User.Id);
            ViewBag.UserType = userType;
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = new CRMProductRepository().GetNames() });
            setting.Add(new SettingExtra() { Key = "UserId", Value = SiteContext.Current.User.Id });
            setting.Add(new SettingExtra() { Key = "UserType", Value = userType });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult SaleForecast()
        {

            var type = Request.QueryString["type"] + "";
            var setting = new List<SettingExtra>();
            var userRepo = new UserRepository();
            if (string.IsNullOrWhiteSpace(type))
            {
                type = "saleperson";
            }
            var forecastType = ((CRMSaleForecastType)Enum.Parse(typeof(CRMSaleForecastType), type, true));
            if (forecastType == CRMSaleForecastType.SalePerson)
            {
                var isSalePerson = userRepo.CheckUserType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
                setting.Add(new SettingExtra() { Key = "IsSalePerson", Value = isSalePerson });
                if (isSalePerson)
                {
                    setting.Add(new SettingExtra() { Key = "SalePersons", Value = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id) });
                }
                else
                {
                    setting.Add(new SettingExtra() { Key = "SalePersons", Value = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive) });
                }
            }
            if (forecastType == CRMSaleForecastType.RSM)
            {
            }
            else if (forecastType == CRMSaleForecastType.Summary)
            {
            }
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            setting.Add(new SettingExtra() { Key = "Products", Value = new CRMProductRepository().GetOwnProductsIdName() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult DHSaleForecast()
        {
            return View();
        }
        public ActionResult RSMSaleForecast()
        {

            var type = Request.QueryString["type"] + "";
            var setting = new List<SettingExtra>();
            var userRepo = new UserRepository();
            if (string.IsNullOrWhiteSpace(type))
            {
                type = "saleperson";
            }
            var forecastType = ((CRMSaleForecastType)Enum.Parse(typeof(CRMSaleForecastType), type, true));
            if (forecastType == CRMSaleForecastType.SalePerson)
            {
                var isSalePerson = userRepo.CheckUserType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
                setting.Add(new SettingExtra() { Key = "IsSalePerson", Value = isSalePerson });
                if (isSalePerson)
                {
                    setting.Add(new SettingExtra() { Key = "SalePersons", Value = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id) });
                }
                else
                {
                    setting.Add(new SettingExtra() { Key = "SalePersons", Value = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive) });
                }
            }
            if (forecastType == CRMSaleForecastType.RSM)
            {
            }
            else if (forecastType == CRMSaleForecastType.Summary)
            {
            }
            setting.Add(new SettingExtra() { Key = "Products", Value = new CRMProductRepository().GetOwnProductsIdName() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View();
        }
        public ActionResult Priorities()
        {
            return View();
        }
        public ActionResult ComplaintStatuses()
        {
            return View();
        }
        public ActionResult CaseTypes()
        {
            return View();
        }
        public ActionResult ComplaintTests()
        {
            return View();
        }
        public ActionResult ComplaintLabs()
        {
            return View();
        }
        public ActionResult AdminTasks()
        {
            return View();
        }
        public ActionResult GoogleCalendarIntegration()
        {
            var userRepo = new UserRepository();
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.EventModes = new GenericRepository<EventMode>().GetNames();
            ViewBag.EventStatuses = new GenericRepository<EventStatus>().GetNames("Name", "EventStatuses");
            ViewBag.Statuses = new GenericRepository<ProductStatus>().GetNames("Name", "ProductStatuses");
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetProductsIdName(SiteContext.Current.UserTypeId);
            ViewBag.LostReasons = new GenericRepository<ProductLostReason>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            ViewBag.Fiscals = new FiscalRepository().GetNames();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Products", Value = new CRMProductRepository().GetOwnProductsIdName() });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }

        #region Reports
        public ActionResult SaleSummaryByDate()
        {
            var userRepo = new UserRepository();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Products = new CRMProductRepository().GetNames(SiteContext.Current.UserTypeId);
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            return View("~/Views/CRM/Reports/salesummarybydate.cshtml");
        }
        public ActionResult SaleSummaryByCustomerProduct()
        {
            var userRepo = new UserRepository();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Customers = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId);
            ViewBag.Products = new CRMProductRepository().GetNames(SiteContext.Current.UserTypeId);
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            return View("~/Views/CRM/Reports/SaleSummaryByCustomerProduct.cshtml");
        }
        public ActionResult SaleComparisonByYear()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "UserTypeId", Value = SiteContext.Current.UserTypeId });
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/CRM/Reports/SaleComparisonByYear.cshtml");
        }

        public ActionResult salecomparisonbyyearbysaletype()
        {
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "UserTypeId", Value = SiteContext.Current.UserTypeId });
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/CRM/Reports/salecomparisonbyyearbysaletype.cshtml");
        }


        
        public ActionResult SaleSummaryProductWise()
        {
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            return View("~/Views/CRM/Reports/salesummaryproductwise.cshtml");
        }
        public ActionResult Marketsharesituation()
        {
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            return View("~/Views/CRM/Reports/marketsharesituation.cshtml");
        }
        public ActionResult CustomerAndDivisionWiseSaleSummaryDetail()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/CustomerAndDivisionWiseSaleSummaryDetail.cshtml");
        }
        public ActionResult SaleForecastbyProductCustomer()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Categories = new GenericRepository<ProductCategory>().GetNames();
            ViewBag.SecCategories = new GenericRepository<ProductSecCategory>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "ViewProductSecondaryCategory", Value = ActionManager.ViewProductSecondaryCategory });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/CRM/Reports/saleforecastbyproductcustomer.cshtml");
        }
        public ActionResult SaleForecastSummaryRegionWise()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Categories = new GenericRepository<ProductCategory>().GetNames();
            ViewBag.SecCategories = new GenericRepository<ProductSecCategory>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "ViewProductSecondaryCategory", Value = ActionManager.ViewProductSecondaryCategory });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View("~/Views/CRM/Reports/saleforecastsummaryregionwise.cshtml");

        }



        public ActionResult salepersonwiseSaleSummaryDetail()
        {
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            return View("~/Views/CRM/Reports/salepersonwiseSaleSummaryDetail.cshtml");
        }
        public ActionResult YearWiseMonthlySaleComparison()
        {

            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            return View("~/Views/CRM/Reports/YearWiseMonthlySaleComparison.cshtml");
        }
        public ActionResult DivisionWiseMonthlySaleComparison()
        {
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/DivisionWiseMonthlySaleComparison.cshtml");
        }
        public ActionResult SalePersonWiseMonthlySaleComparison()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            return View("~/Views/CRM/Reports/SalePersonWiseMonthlySaleComparison.cshtml");
        }

        public ActionResult RegionAndDivisionWiseMonthlySaleComparison()
        {
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            return View("~/Views/CRM/Reports/RegionAndDivisionWiseMonthlySaleComparison.cshtml");
        }
        public ActionResult CustomerAndDivisionWiseYearlySaleComparison()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/CustomerAndDivisionWiseYearlySaleComparison.cshtml");
        }
        public ActionResult CustomerWisePotential()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View("~/Views/CRM/Reports/customerwisepotential.cshtml");
        }
        public ActionResult RegionWisePotential()
        {
            return View("~/Views/CRM/Reports/Regionwisepotential.cshtml");
        }
        public ActionResult SalePersonWisePotential()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            var userRepo = new UserRepository();
            var isSalePerson = userRepo.CheckUserType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
            if (isSalePerson)
            {
                ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive, SiteContext.Current.User.Id);
            }
            else
            {
                ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            }

            return View("~/Views/CRM/Reports/SalePersonWisepotential.cshtml");
        }

        public ActionResult ProductWisePotential()
        {
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Vendors = new GenericRepository<CRMVendor>().GetNames();
            ViewBag.Currencies = new CurrencyRepository().GetNames();
            return View("~/Views/CRM/Reports/Productwisepotential.cshtml");
        }
        public ActionResult OrganizationNDivisionWiseProjectDetail()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/OrganizationNDivisionWiseProjectDetail.cshtml");
        }
        public ActionResult SalePersonNDivisionWiseProjectDetail()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/SalePersonNDivisionWiseProjectDetail.cshtml");
        }

        public ActionResult RegionNDivisionWiseProjectDetail()
        {
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/RegionNDivisionWiseProjectDetail.cshtml");
        }

        public ActionResult ProjectDetail()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/projectdetail.cshtml");
        }
        public ActionResult OwnershipTransfer()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = userRepo.LoadCRMUserByType(CRMUserType.SalesExecutive);
            return View();
        }
        public ActionResult ProjectDetailProductWise()
        {
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            return View("~/Views/CRM/Reports/projectdetailproductwise.cshtml");
        }

        public ActionResult VisitDetailByDate()
        {
            return View("~/Views/CRM/Reports/visitdetailbydate.cshtml");
        }
        public ActionResult VisitDetailBySalePerson()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Projects = new CRMProjectRepository().GetProjectsByUserTypeId();
            return View("~/Views/CRM/Reports/visitdetailbysaleperson.cshtml");
        }
        public ActionResult VisitDetailByCustomer()
        {
            ViewBag.Projects = new CRMProjectRepository().GetProjectsByUserTypeId();
            return View("~/Views/CRM/Reports/visitdetailbycustomer.cshtml");
        }
        public ActionResult VisitCountByCustomer()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Projects = new CRMProjectRepository().GetProjectsByUserTypeId();
            return View("~/Views/CRM/Reports/visitcountbycustomer.cshtml");
        }
        public ActionResult CustomerWiseMonthlySale()
        {
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();
            return View("~/Views/CRM/Reports/CustomerWiseMonthlySale.cshtml");
        }
        public ActionResult ProductWiseMonthlySale()
        {
            var userRepo = new UserRepository();
            ViewBag.SalePersons = new UserRepository().LoadCRMUserByType(CRMUserType.SalesExecutive);
            ViewBag.Regions = new GenericRepository<Region>().GetNames();
            ViewBag.Divisions = new GenericRepository<Division>().GetNames();
            ViewBag.Products = new CRMProductRepository().GetNames(SiteContext.Current.UserTypeId);
            ViewBag.Groups = new GenericRepository<ProductGroup>().GetNames();
            ViewBag.Industries = new GenericRepository<Industry>().GetNames();


            return View("~/Views/CRM/Reports/ProductWiseMonthlySale.cshtml");
        }
        public ActionResult InvoicePerformaDateWise()
        {
            return View("~/Views/CRM/Reports/Invoiceperformadatewise.cshtml");
        }
        public ActionResult TestingResults()
        {
            return View("~/Views/CRM/Reports/testingresults.cshtml");
        }
        public ActionResult MergeProducts()
        {
            return View("~/Views/CRM/mergeproducts.cshtml");
        }
        #endregion







    }
}
