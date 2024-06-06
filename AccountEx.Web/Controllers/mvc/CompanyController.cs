using System.Web.Mvc;
using AccountEx.Repositories;
using AccountEx.Common;
using AccountEx.BussinessLogic;
using System.Linq;

namespace AccountEx.Web.Controllers.mvc
{
    public class CompanyController : BaseController
    {
        public ActionResult Index()
        {
            var userCompanies = new UserCompanyRepository().GetAll(e => e.UserId == SiteContext.Current.User.Id).Select(a => a.CompanyId);
            // Get User Created Companies, User Allowed Companies, User Default Company
            var userAllowedCompanies = new UserCompanyRepository().GetAll(e => e.UserId == SiteContext.Current.User.Id || e.CompanyId == SiteContext.Current.User.CompanyId)
                .Select(a => a.AuthCompanyId)
                .ToList();

            ViewBag.Companies = new CompanyRepository()
                .GetAll(e => 
                e.CreatedBy == SiteContext.Current.User.Id 
                || userCompanies.Contains(e.Id)
                || userAllowedCompanies.Contains(e.Id)
                || e.Id == SiteContext.Current.User.CompanyId);

            return View();
        }
        public ActionResult Companies()
        {
            ViewBag.Companies = new CompanyRepository().GetAll();
            return View();
        }
        public ActionResult UserCompanies()
        {
            var userCompanies = new UserCompanyRepository().GetAll(e => e.UserId == SiteContext.Current.User.Id).Select(a => a.AuthCompanyId).ToList();
            // Get User Created Companies, User Allowed Companies, User Default Company
            ViewBag.Companies = new CompanyRepository()
                .GetAll(e =>
                e.CreatedBy == SiteContext.Current.User.Id
                || userCompanies.Contains(e.Id)
                || e.Id == SiteContext.Current.User.CompanyId);

            // insert deafult company id
            userCompanies.Add(SiteContext.Current.User.CompanyId);
            ViewBag.Users = new UserRepository().GetAllUsersByCompanyIds(userCompanies);
            return View();
        }
        public ActionResult SalaryList()
        {
            return View();
        }

        //public void SwitchCompany(int compnayId)
        //{
        //    var company = new CompanyRepository().GetById(compnayId);
        //    var user = new UserRepository().GetById(SiteContext.Current.User.Id);
        //    SiteContext.Current.User.CompanyId = compnayId;
        //    SiteContext.Current.UserRoles = new UserRoleRepository().GetRoles(user.Id);
        //    SiteContext.Current.MenuItems = null;
        //    SiteContext.Current.Settings = null;
        //    SiteContext.Current.Fiscal = new FiscalRepository().GetDefaultFiscal();
        //    SettingManager.RefreshSetting();
        //    Response.Redirect("~/Account/dashboard");

        //}
        public void SwitchFiscal(int fiscalId)
        {
            if (SiteContext.Current.User.CanChangeFiscal)
            {
                SiteContext.Current.Fiscal = UtilityFunctionManager.GetFiscalForSiteContext(new FiscalRepository().GetById(fiscalId));
                var userRepo = new UserRepository();
                var user = userRepo.GetByUsername(SiteContext.Current.User.Username);
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
            }
            var url = HttpContext.Request.UrlReferrer.OriginalString;
            if (string.IsNullOrWhiteSpace(url))
                url = "~/home/xdashboard?name=AdminDashboard?name=AdminDashboard";
            //if (!url.StartsWith("~"))
            //    url = "~" + url;
            if (!Response.IsRequestBeingRedirected)
                Response.Redirect(url);

        }

    }
}
