using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AccountEx.Common;
using AccountEx.Web.Controllers.api;

namespace AccountEx.Web.Filters
{
    public sealed class SessionManagementAttribute : ActionFilterAttribute
    {
        public SessionManagementAttribute()
        {
            //  SessionFactory = WebApiApplication.SessionFactory;
        }

        // private ISessionFactory SessionFactory { get; set; }


        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string file = HttpContext.Current.Request.Url.Segments[HttpContext.Current.Request.Url.Segments.Length - 1].ToLower();
        //    var returntype = (((System.Web.Mvc.ReflectedActionDescriptor)(filterContext.ActionDescriptor)).MethodInfo).ReturnType.Name;
        //    if (returntype.ToLower() == "actionresult")
        //    {
        //        //new MenuItemRepository()
        //        List<string> sessionlessPages = new List<string> { "login", "ajaxlogin", "checklogin", "logoff", "unauthorize" };
        //        List<string> allowedPages = new List<string> { "/", "changepassword", "change-password", "profile", "settings", "commoditysetups", "CustomerJobs", "usersettings" };
        //        allowedPages = allowedPages.ConvertAll(p => p.ToLower());
        //        if (!sessionlessPages.Contains(file.ToLower()))
        //        {
        //            if (SiteContext.Current.User == null)
        //            {
        //                HttpContext.Current.Response.Redirect("~/Account/ajaxlogin?ReturnURL=" + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.ToString()));
        //                return;
        //            }
        //            else
        //            {
        //                var absolutePath = HttpContext.Current.Request.Url.AbsolutePath.ToLower();
        //                var virtualDirectory = (ConfigurationManager.AppSettings["VirtualDirectory"] + "").ToLower();
        //                if (virtualDirectory != "") absolutePath = absolutePath.Replace(virtualDirectory + "/", "");
        //                if (absolutePath == "" || allowedPages.Contains(file.ToLower()))
        //                    SiteContext.Current.RoleAccess = new RoleAccess() { CanView = true, CanCreate = true, CanDelete = true, CanUpdate = true, CanAuthorize = true };
        //                else
        //                    SiteContext.Current.RoleAccess = MenuManager.GetMenuAccess(absolutePath);
        //                if (SiteContext.Current.RoleAccess == null || !SiteContext.Current.RoleAccess.CanView)
        //                {
        //                    HttpContext.Current.Response.Redirect("~/Account/Unauthorize?abUrl=" + HttpContext.Current.Request.Url.AbsolutePath + "&abPath=" + absolutePath);
        //                    //Response.Redirect("~/Account/Unauthorize");
        //                    return;
        //                }
        //                base.OnActionExecuting(filterContext);
        //            }
        //        }
        //        else
        //            base.OnActionExecuting(filterContext);

        //    }
        //}



        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            //actionContext.http
            // if (filterContext.HttpContext.Request.IsAjaxRequest())
            // {
            //     filterContext.HttpContext.Response.StatusCode = 403;
            //     filterContext.HttpContext.Response.End();

            // }

            if (SiteContext.Current.User == null)
            {
                //var response = new ApiResponse() { Success = true, Data = "Session has expired", };
                
                // Temporarily check added to allow NexusSyncApiController to receive request
                // Later it will be handled properly

                if (actionContext.ControllerContext.Controller.GetType() != typeof(NexusSyncApiController))
                    throw new HttpResponseException(System.Net.HttpStatusCode.TemporaryRedirect);
            }
        
        
        
       }




        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
        }
    }
}