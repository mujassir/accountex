using System.Collections.Generic;
using System.Web.Mvc;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System.Configuration;
using AccountEx.BussinessLogic;
using AccountEx.Web.Code;

namespace AccountEx.Web.Controllers.mvc
{
    [Authorize]
    public class BaseController : Controller
    {
        public static string CultureName { get; set; }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string file = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLower();

            //var IsSiteDown = ConfigurationReader.GetConfigKeyValue<bool>("IsSiteDown");
            //if (IsSiteDown && !file.Contains("sitemaintenance"))
            //{
            //    Response.Redirect("~/maintenance/sitemaintenance", true);
            //}
            //else
            //{


            var returntype = (((System.Web.Mvc.ReflectedActionDescriptor)(filterContext.ActionDescriptor)).MethodInfo).ReturnType.Name;
            if (returntype.ToLower() == "actionresult")
            {
                //new MenuItemRepository()
                List<string> sessionlessPages = new List<string> { "login", "ajaxlogin", "checklogin", "logoff", "unauthorize", "sitemaintenance" };
                List<string> allowedPages = new List<string> { "/", "changepassword", "change-password", "profile", "settings", "customerjobs", "settings", "menumanagement", "sitemaintenance" };
                allowedPages = allowedPages.ConvertAll(p => p.ToLower());
                if (!sessionlessPages.Contains(file.ToLower()))
                {
                    if (SiteContext.Current.User == null)
                    {
                        if (!Response.IsRequestBeingRedirected)
                            Response.Redirect("~/Account/login?ReturnURL=" + Server.UrlEncode(Request.Url.ToString()));
                        return;
                    }
                    else
                    {
                        var absolutePath = Request.Url.AbsolutePath.ToLower();
                        if (!string.IsNullOrWhiteSpace(Request["type"]))
                        {
                            absolutePath = absolutePath + "?type=" + Request["type"];
                        }
                        if (!string.IsNullOrWhiteSpace(Request["format"]))
                        {
                            if (!string.IsNullOrWhiteSpace(Request["type"]))
                            {
                                absolutePath = absolutePath + "&format=" + Request["format"];
                            }
                            else
                            {
                                absolutePath = absolutePath + "?format=" + Request["format"];
                            }
                        }
                        var virtualDirectory = (ConfigurationManager.AppSettings["VirtualDirectory"] + "").ToLower();
                        if (virtualDirectory != "") absolutePath = absolutePath.Replace(virtualDirectory + "/", "");
                        //if (absolutePath == "" || allowedPages.Contains(file.ToLower()) || SiteContext.Current.User.IsAdmin)
                        if (absolutePath == "" || allowedPages.Contains(file.ToLower()) || SiteContext.Current.User.IsAdmin)
                            SiteContext.Current.RoleAccess = UtilityFunctionManager.GetRoleAccessForSiteContext(new RoleAccess() { CanView = true, CanCreate = true, CanDelete = true, CanUpdate = true, CanAuthorize = true });
                        else
                        {
                            //SiteContext.Current.RoleAccess = new RoleAccess() { CanView = true, CanCreate = true, CanDelete = true, CanUpdate = true, CanAuthorize = true };
                            SiteContext.Current.RoleAccess = UtilityFunctionManager.GetRoleAccessForSiteContext(MenuManager.GetMenuAccess(absolutePath));
                        }
                        if (SiteContext.Current.RoleAccess == null || !SiteContext.Current.RoleAccess.CanView)
                        {
                            if (!Response.IsRequestBeingRedirected)
                                Response.Redirect("~/Account/Unauthorize?abUrl=" + Request.Url.AbsolutePath + "&abPath=" + absolutePath);
                            //Response.Redirect("~/Account/Unauthorize");
                            return;
                        }
                        base.OnActionExecuting(filterContext);
                    }
                }
                else
                    base.OnActionExecuting(filterContext);
                // }

            }
        }


        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var user = SiteContext.Current.User;
        //    var file = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLower();
        //    var id = Session["ID"] + "";
        //    var allowedUrls = new List<string> { "login", "ajax-login", "checklogin", "logoff", "MenuManagement" };
        //    SiteContext.Current.RoleAccess = new RoleAccess()
        //    {
        //        CanAuthorize=true,
        //        CanCreate=true,
        //        CanView=true,
        //        CanUpdate=true,
        //        CanDelete=true
        //    };
        //    if (!allowedUrls.Contains(file.ToLower()))
        //    {
        //        if (SiteContext.Current.User == null)
        //        {
        //            Response.Redirect("~/Account/login?ReturnURL=" + Server.UrlEncode(Request.Url.ToString()));
        //            return;
        //        }
        //        else
        //            base.OnActionExecuting(filterContext);
        //    }
        //    else
        //        base.OnActionExecuting(filterContext);

        //}
    }
}




