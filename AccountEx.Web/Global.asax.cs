using AccountEx.Web.Filters;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using AccountEx.Repositories;
using AccountEx.Common;
using System;
namespace AccountEx.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.Contains("api");
        }

        public static string ApplicationName;
        protected void Application_Start()
        {

            // Just done for VEX for following error
            // Unable to find assembly 'EntityFrameworkDynamicProxies-AccountEx.CodeFirst, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.

            var user = new UserRepository().FirstOrDefault();

            // Just done for VEX


            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "dd/MM/yyyy"  });
            //json.SerializerSettings.Converters.Add(new MyDateTimeConvertor());
            GlobalConfiguration.Configuration.Filters.Add(new SessionManagementAttribute());
            MvcHandler.DisableMvcResponseHeader = true;
            //HangfireBootstrapper.Instance.Start();
            // AutoMapperConfiguration.Configure();
        }


        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {

            HttpContext.Current.Response.Headers.Remove("Server");
        }
        protected void Application_BeginRequest()
        {

            //InitApplication();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Response.Filter = null;
        }
        protected void Application_End(object sender, EventArgs e)
        {
          //  HangfireBootstrapper.Instance.Stop();
        }
    }
}