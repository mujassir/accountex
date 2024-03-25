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
using AccountEx.BussinessLogic.Security;
using System.Text;

namespace AccountEx.Web.Controllers.mvc
{
    //[InitializeSimpleMembership]
    [Compress]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login



        [AllowAnonymous]
        [OutputCache(CacheProfile = "Login")]
        public ActionResult Login()
        {


            //var setting = new List<Setting>();
            //setting.Add(new Setting() { Key = "DahhboardUrl", Value = SettingManager.Get("Other.DashBoardUrl") });
            //ViewBag.Settings = JsonConvert.SerializeObject(setting.ToList()); ;
            //SettingManager.RefreshSetting();
            //var settings = new SettingRepository().GetByKeys(new List<string>() { "Application.LoginLogoUrl", "Application.Title" });
            //ViewBag
            //foreach (var item in settings)
            //{
            //    SiteContext.Current.Settings.Add(item.Key, item.Value);
            //}
            var host = Request.Url.Host.ToLower().Replace("www", "").Replace("com", "").Replace(".", "");
            var company = new CompanyRepository().GetByDomainName(host);
            if (company != null)
            {
                var keys = new List<string> { "Application.Title", "Application.LoginLogoUrl" };
                var settings = new SettingRepository().GetByCompanyId(company.Id);
                if (settings.Any(p => p.Key == "Application.Title"))
                    Response.Cookies.Add(new HttpCookie("ApplicationTitle", settings.FirstOrDefault(p => p.Key == "Application.Title").Value) { Expires = DateTime.Now.AddMonths(1) });
                if (settings.Any(p => p.Key == "Application.LoginLogoUrl"))
                    Response.Cookies.Add(new HttpCookie("LoginLogo", settings.FirstOrDefault(p => p.Key == "Application.LoginLogoUrl").Value) { Expires = DateTime.Now.AddMonths(1) });

                Response.Cookies.Add(new HttpCookie("UploadFolder", company.UploadFolder) { Expires = DateTime.Now.AddMonths(1) });

              
            }
            var clientIPAddress = UtilityFunctionManager.GetClientIpAddress(HttpContext.Request);
            Response.Cookies.Add(new HttpCookie("ClientIpAddess", clientIPAddress) { Expires = DateTime.Now.AddDays(2) });
            return View();

        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult Sync()
        {
            //VoucherManager.SyncData();
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        //Old User Management


        //public ActionResult Users()
        //{
        //    ViewBag.Menu = JsonConvert.SerializeObject(MenuManager.GetMenuItems(false));
        //    ViewBag.Customers = new AccountDetailRepository().GetNamesWithoutCode(AccountDetailFormType.Customers);
        //    return View();
        //}
        public ActionResult Users()
        {
            ViewBag.Roles = new GenericRepository<Role>().GetNames();
            ViewBag.Menu = JsonConvert.SerializeObject(MenuManager.GetMenuItems(false));
            if (SettingManager.IsAllowVehicleValidation)
            {
                ViewBag.VehicleBranches = new VehicleBranchRepository().GetNames();
            }

            return View();
        }
        public ActionResult Roles()
        {
            ViewBag.Menu = JsonConvert.SerializeObject(MenuManager.GetMenuItems(false));
            ViewBag.Roles = new GenericRepository<Role>().GetNames();
            ViewBag.DashBoards = DashboardManager.GetAvailableDashBoard();
            return View();
        }
        public ActionResult Manage()
        {
            ViewBag.AttributeTypes = JsonConvert.SerializeObject(new AttributeTypeRepository().GetAll());
            return View();
        }
        public ActionResult AccountAttributes()
        {
            ViewBag.AccountTypes = new AccountTypeRepository().GetAll();
            ViewBag.AttributeTypes = new AttributeTypeRepository().GetNames();
            return View();
        }
        public ActionResult AttributeTypes()
        {
            return View();
        }
        public ActionResult AccountTypes()
        {
            ViewBag.Accounts = new AccountRepository().GetAccountTreeByLevel(3);
            return View();
        }

        public ActionResult Menu()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("DashBoard");
            var bank = 0;
            var customer = 0;
            var cash = 0;
            var suppllier = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var banksetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (banksetting != null)
                bank = Numerics.GetInt(banksetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var cashsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (cashsetting != null)
                cash = Numerics.GetInt(cashsetting.Value);
            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");
            if (suppliersetting != null)
                suppllier = Numerics.GetInt(suppliersetting.Value);
            ViewBag.CashInHand = new OpeningBalanceRepository().GetOpeningBalance(cash, DateTime.Now.Date);
            ViewBag.CashInBank = new OpeningBalanceRepository().GetOpeningBalance(bank, DateTime.Now.Date);
            ViewBag.Sale = new OpeningBalanceRepository().GetSumOnly("Sale", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Purchase = new OpeningBalanceRepository().GetSumOnly("Purchase", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Debitors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, customer);
            ViewBag.Creditors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, suppllier);
            return View();
        }

        [OutputCache(CacheProfile = "Dashboard")]
        public ActionResult Dashboard()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("DashBoard");
            var bank = 0;
            var customer = 0;
            var cash = 0;
            var suppllier = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var banksetting = setting.FirstOrDefault(p => p.KeyName == "BankAccountId");
            if (banksetting != null)
                bank = Numerics.GetInt(banksetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var cashsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (cashsetting != null)
                cash = Numerics.GetInt(cashsetting.Value);
            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");
            if (suppliersetting != null)
                suppllier = Numerics.GetInt(suppliersetting.Value);
            var incomTaxId = AccountManager.GetLeafAccountId("Income Tax");
            var whtId = AccountManager.GetLeafAccountId("WHT");
            var GstId = AccountManager.GetLeafAccountId("GST");
            var transRepo = new TransactionRepository();

            var taxes = transRepo.GetSummary(incomTaxId) + transRepo.GetSummary(whtId) + transRepo.GetSummary(GstId);
            ViewBag.Taxes = Numerics.DecimalToString(taxes, 0);
            //ViewBag.GST = Numerics.DecimalToString(new TransactionRepository().GetSummary(GstId), 0);
            //ViewBag.WHT = Numerics.DecimalToString(new TransactionRepository().GetSummary(whtId), 0);
            //ViewBag.CashInHand = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankReceipts), 0);

            ViewBag.CashInBank = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankPayments, VoucherType.BankReceipts), 0);

            //ViewBag.CashInBank = 0.00;

            ViewBag.Sale = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Sale), 0); //new OpeningBalanceRepository().GetSumOnly("Sale", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Purchase = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Purchase), 0); //new OpeningBalanceRepository().GetSumOnly("Purchase", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Debitors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, customer);
            ViewBag.Creditors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, suppllier);



            return View();
        }
        [OutputCache(CacheProfile = "Dashboard")]
        public ActionResult Dashboard2()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("DashBoard");
            var bank = 0;
            var customer = 0;
            var cash = 0;
            var suppllier = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var banksetting = setting.FirstOrDefault(p => p.KeyName == "BankAccountId");
            if (banksetting != null)
                bank = Numerics.GetInt(banksetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var cashsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (cashsetting != null)
                cash = Numerics.GetInt(cashsetting.Value);
            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");
            if (suppliersetting != null)
                suppllier = Numerics.GetInt(suppliersetting.Value);
            var incomTaxId = AccountManager.GetLeafAccountId("Income Tax");
            var whtId = AccountManager.GetLeafAccountId("WHT");
            var GstId = AccountManager.GetLeafAccountId("GST");
            var transRepo = new TransactionRepository();
            var taxes = transRepo.GetSummary(incomTaxId) + transRepo.GetSummary(whtId) + transRepo.GetSummary(GstId);
            ViewBag.Taxes = Numerics.DecimalToString(taxes, 0);
            //ViewBag.GST = Numerics.DecimalToString(new TransactionRepository().GetSummary(GstId), 0);
            //ViewBag.WHT = Numerics.DecimalToString(new TransactionRepository().GetSummary(whtId), 0);
            //ViewBag.CashInHand = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankReceipts), 0);

            ViewBag.CashInBank = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankPayments, VoucherType.BankReceipts), 0);

            //ViewBag.CashInBank = 0.00;

            ViewBag.Sale = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Sale), 0); //new OpeningBalanceRepository().GetSumOnly("Sale", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Purchase = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Purchase), 0); //new OpeningBalanceRepository().GetSumOnly("Purchase", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Debitors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, customer);
            ViewBag.Creditors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, suppllier);



            return View();
        }
        [OutputCache(CacheProfile = "Dashboard")]
        public ActionResult Dashboard4()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("DashBoard");
            var bank = 0;
            var customer = 0;
            var cash = 0;
            var suppllier = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var banksetting = setting.FirstOrDefault(p => p.KeyName == "BankAccountId");
            if (banksetting != null)
                bank = Numerics.GetInt(banksetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var cashsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (cashsetting != null)
                cash = Numerics.GetInt(cashsetting.Value);
            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");
            if (suppliersetting != null)
                suppllier = Numerics.GetInt(suppliersetting.Value);
            var incomTaxId = AccountManager.GetLeafAccountId("Income Tax");
            var whtId = AccountManager.GetLeafAccountId("WHT");
            var GstId = AccountManager.GetLeafAccountId("GST");
            var transRepo = new TransactionRepository();
            var taxes = transRepo.GetSummary(incomTaxId) + transRepo.GetSummary(whtId) + transRepo.GetSummary(GstId);
            ViewBag.Taxes = Numerics.DecimalToString(taxes, 0);
            //ViewBag.GST = Numerics.DecimalToString(new TransactionRepository().GetSummary(GstId), 0);
            //ViewBag.WHT = Numerics.DecimalToString(new TransactionRepository().GetSummary(whtId), 0);
            //ViewBag.CashInHand = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankReceipts), 0);

            ViewBag.CashInBank = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankPayments, VoucherType.BankReceipts), 0);

            //ViewBag.CashInBank = 0.00;

            ViewBag.Sale = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Sale), 0); //new OpeningBalanceRepository().GetSumOnly("Sale", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Purchase = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Purchase), 0); //new OpeningBalanceRepository().GetSumOnly("Purchase", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Debitors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, customer);
            ViewBag.Creditors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, suppllier);



            return View();
        }

        public ActionResult Dashboard5()
        {
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("DashBoard");
            var bank = 0;
            var customer = 0;
            var cash = 0;
            var suppllier = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            var banksetting = setting.FirstOrDefault(p => p.KeyName == "BankAccountId");
            if (banksetting != null)
                bank = Numerics.GetInt(banksetting.Value);
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var cashsetting = setting.FirstOrDefault(p => p.KeyName == "ItemAccountId");
            if (cashsetting != null)
                cash = Numerics.GetInt(cashsetting.Value);
            var suppliersetting = setting.FirstOrDefault(p => p.KeyName == "SupplierAccountId");
            if (suppliersetting != null)
                suppllier = Numerics.GetInt(suppliersetting.Value);
            var incomTaxId = AccountManager.GetLeafAccountId("Income Tax");
            var whtId = AccountManager.GetLeafAccountId("WHT");
            var GstId = AccountManager.GetLeafAccountId("GST");
            var transRepo = new TransactionRepository();
            var taxes = transRepo.GetSummary(incomTaxId) + transRepo.GetSummary(whtId) + transRepo.GetSummary(GstId);
            ViewBag.Taxes = Numerics.DecimalToString(taxes, 0);
            //ViewBag.GST = Numerics.DecimalToString(new TransactionRepository().GetSummary(GstId), 0);
            //ViewBag.WHT = Numerics.DecimalToString(new TransactionRepository().GetSummary(whtId), 0);
            //ViewBag.CashInHand = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankReceipts), 0);

            ViewBag.CashInBank = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.BankPayments, VoucherType.BankReceipts), 0);

            //ViewBag.CashInBank = 0.00;

            ViewBag.Sale = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Sale), 0); //new OpeningBalanceRepository().GetSumOnly("Sale", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Purchase = Numerics.DecimalToString(new TransactionRepository().GetSummary(VoucherType.Purchase), 0); //new OpeningBalanceRepository().GetSumOnly("Purchase", DateTime.Now, (byte)EntryType.MasterDetail);
            ViewBag.Debitors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, customer);
            ViewBag.Creditors = new OpeningBalanceRepository().GetOpeningBalance(DateTime.Now, suppllier);
            return View();
        }
        public string ValidateLogin()
        {
            ApiResponse response;
            try
            {
                var username = Request["UserName"];
                var password = Request["Password"];
                //var user = new UserRepository().GetByUsername(username);
                //username.ToLower() == "admin" &&
                if (password == SettingManager.DeletePassword)
                {
                    //var hash = Sha1Sign.Hash(user.Username + password);
                    //if (user.Hash == hash)
                    //{
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = ""

                    };
                }
                else
                {

                    response = new ApiResponse() { Success = false, Error = "Password provided is incorrect." };
                }


                //}
                //else
                //{

                //    response = new ApiResponse() { Success = false, Error = "The user name or password provided is incorrect." };
                //}

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return JsonConvert.SerializeObject(response);



        }

        [AllowAnonymous]
        public string CheckLogin()
        {
            ApiResponse response;
            try
            {
                var username = (Request["Username"] + "").ToLower();
                var password = Request["Password"];
                var lastDate = Request["LastDate"];
                var isRememebr = Numerics.GetBool(Request["IsRemember"]);
                var isGlobalLogin = Numerics.GetBool(Request["IsGlobalLogin"]);
                var fiscalId = Numerics.GetInt(Request["FiscalId"]);
                bool loadFiscals = true;
                var host = Request.Url.Host.ToLower().Replace("www", "").Replace("com", "").Replace(".", "");
                var companyRepo = new CompanyRepository();
                var company = companyRepo.GetByDomainName(host);
                var userRepo = new UserRepository();
                var userRoleRepo = new UserRoleRepository(userRepo);
                var user = userRepo.GetByUsername(username);
                if (user != null)
                {
                    if (company != null && !company.AllowOtherLogin && company.Id != user.CompanyId)
                    {
                        response = new ApiResponse() { Success = false, Error = "Username or Password provided is incorrect." };
                    }
                    else
                    {
                        var hash = Sha1Sign.Hash(username + password);
                        if (user.Hash == hash)
                        {
                            //Clear all cookies
                            if (Request.Cookies["UserName"] != null)
                            {
                                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies["UploadFolder"].Expires = DateTime.Now.AddDays(-1);


                            }
                            Session["UserId"] = user.Id;
                            SiteContext.Current.LogMappings = UtilityFunctionManager.GetLogMappingForSiteContext(new LogMappingRepository().GetAllMapping());
                            SiteContext.Current.RootPath = Server.MapPath("../");
                            SiteContext.Current.UploadFolder = companyRepo.GetUploadFolder(user.CompanyId.Value);
                            SiteContext.Current.UserCompany = Numerics.GetInt(user.CompanyId);
                            //if (!user.CanChangeFiscal || isGlobalLogin || new FiscalRepository().GetTotalFiscal(Numerics.GetInt(user.CompanyId)) == 1)
                            //{

                            FormsAuthentication.SetAuthCookie(username, false);
                            SiteContext.Current.User = UtilityFunctionManager.GetUserForSiteContext(user);
                            SiteContext.Current.UserRoles = new UserRoleRepository().GetRoles(user.Id);
                            SiteContext.Current.InfoText = companyRepo.GetInfoText(Numerics.GetInt(user.CompanyId));
                            if (isGlobalLogin)
                            {
                                SiteContext.Current.Fiscal = UtilityFunctionManager.GetFiscalForSiteContext(new FiscalRepository().GetById(fiscalId));
                                SiteContext.Current.RoleAccess = UtilityFunctionManager.GetRoleAccessForSiteContext(GetAccess());
                            }
                            else
                                SiteContext.Current.Fiscal = UtilityFunctionManager.GetFiscalForSiteContext(new FiscalRepository().GetDefaultFiscal());
                            SiteContext.Current.Settings = null;
                            SettingManager.RefreshSetting();
                            loadFiscals = false;
                            //}
                            Response.Cookies.Add(new HttpCookie("ApplicationTitle", SettingManager.ApplicationTitle) { Expires = DateTime.Now.AddMonths(1) });
                            Response.Cookies.Add(new HttpCookie("LoginLogo", SettingManager.LoginLogo) { Expires = DateTime.Now.AddMonths(1) });
                            Response.Cookies.Add(new HttpCookie("UploadFolder", SiteContext.Current.UploadFolder) { Expires = DateTime.Now.AddMonths(1) });
                            if (isRememebr)
                            {
                                Response.Cookies.Add(new HttpCookie("UserName", RsaCrypto.Encrypt(username)) { Expires = DateTime.Now.AddMonths(1) });
                                Response.Cookies.Add(new HttpCookie("Password", RsaCrypto.Encrypt(password)) { Expires = DateTime.Now.AddMonths(1) });
                                ;
                            }
                            if (user.CompanyId == 91)
                            {
                                SiteContext.Current.FiscalSettings = null;
                                FiscalSettingManager.RefreshSetting();
                                var crmUser = new UserRepository().GetCRMUserById(user.Id);
                                SiteContext.Current.UserTypeId = user.UserTypeId.Value;
                                SiteContext.Current.RegionId = Numerics.GetInt(crmUser.RegionId);
                                SiteContext.Current.DivisionId = Numerics.GetInt(crmUser.DivisionId);
                                SiteContext.Current.RSMId = Numerics.GetInt(crmUser.RSMId);

                            }

                            var dashboardUrl = SettingManager.Get("Other.DashBoardUrl");
                            response = new ApiResponse()
                            {
                                Success = true,
                                Data = new
                                {
                                    DashBoardUrl = SettingManager.Get("Other.DashBoardUrl"),
                                    Fiscals = loadFiscals ? new FiscalRepository().GetNames(Numerics.GetInt(user.CompanyId)) : null,
                                    StorageKey = Sha2Sign.Hash(user.CompanyId)
                                }
                            };
                        }
                        else
                            response = new ApiResponse() { Success = false, Error = "Username or Password provided is incorrect." };
                    }
                }
                else
                {

                    response = new ApiResponse() { Success = false, Error = "Username or Password provided is incorrect." };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return JsonConvert.SerializeObject(response);
        }

        private RoleAccess GetAccess()
        {

            string file = Request["url"];
            file = file.Split('/').LastOrDefault();
            List<string> sessionlessPages = new List<string> { "login", "ajaxlogin", "checklogin", "logoff", "unauthorize", "sitemaintenance" };
            List<string> allowedPages = new List<string> { "/", "changepassword", "change-password", "profile", "settings", "customerjobs", "sitesetting", "menumanagement", "sitemaintenance" };
            allowedPages = allowedPages.ConvertAll(p => p.ToLower());
            var access = new RoleAccess() { CanView = true, CanCreate = true, CanDelete = true, CanUpdate = true, CanAuthorize = true };
            if (!sessionlessPages.Contains(file.ToLower()) || !allowedPages.Contains(file.ToLower()))
            {
                var absolutePath = Request.Url.AbsolutePath.ToLower();
                var virtualDirectory = (ConfigurationManager.AppSettings["VirtualDirectory"] + "").ToLower();
                if (virtualDirectory != "") absolutePath = absolutePath.Replace(virtualDirectory + "/", "");
                access = new RoleAccess() { CanView = true, CanCreate = true, CanDelete = true, CanUpdate = true, CanAuthorize = true };
                //access = MenuManager.GetMenuAccess(absolutePath);


            }
            return access;
        }

        [AllowAnonymous]
        public string Continue()
        {
            ApiResponse response;
            try
            {

                var branchId = Numerics.GetInt(Request["FiscalId"]);
                var userId = Numerics.GetInt(Session["UserId"]);
                var user = new UserRepository().GetById(true, userId);
                if (user != null)
                {
                    SiteContext.Current.User = UtilityFunctionManager.GetUserForSiteContext(user);
                    SiteContext.Current.UserRoles = new UserRoleRepository().GetRoles(user.Id);
                    SiteContext.Current.Fiscal = UtilityFunctionManager.GetFiscalForSiteContext(new FiscalRepository().GetById(branchId));
                    FormsAuthentication.SetAuthCookie(user.Username, false);

                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = new
                        {
                            DashBoardUrl = SettingManager.Get("Other.DashBoardUrl"),
                            StorageKey = Sha2Sign.Hash(SiteContext.Current.User.CompanyId)
                        }
                    };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = "you are not allowed for specific action." };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return JsonConvert.SerializeObject(response);



        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult ChartOfAccounts()
        {
            var accountIds = JsonConvert.SerializeObject(new AccountRepository().GetIdsByName(SettingManager.HiddenAccounts));
            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "HiddenAccounts", Value = accountIds });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }

        public ActionResult Coa()
        {
            var typeids = new AccountRepository().GetIdsByName(SettingManager.AccountType);
            ViewBag.HeadAccounts = new AccountRepository().GetByLevel(3, typeids);
            return View();
        }
        //
        // POST: /Account/Login

        //
        // POST: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            SiteContext.Current.User = null;
            // clear authentication cookie
            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            var cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            return RedirectToAction("Login", "Account");
        }


        public ActionResult UnAuthorize()
        {
            return View();
        }

        public string GetNextAccountCode()
        {

            string result;
            try
            {
                var po = AccountManager.GetNextAccountCode();
                result = JsonResult(true, po + "");
            }
            catch (Exception)
            {

                result = JsonResult(false, "");
            }
            return result;




        }
        public string JsonResult(bool success, string data)
        {
            return new JavaScriptSerializer().Serialize(new
            {
                Result = success,
                Data = data,
            });
        }
        //



        #region Helpers

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

        //public enum ManageMessageId
        //{
        //    ChangePasswordSuccess,
        //    SetPasswordSuccess,
        //    RemoveLoginSuccess,
        //}

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                //OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion


    }
}

