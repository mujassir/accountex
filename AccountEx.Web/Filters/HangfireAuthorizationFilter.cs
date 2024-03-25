//using System;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Threading;
//using System.Web.Mvc;
//using WebMatrix.WebData;
//using System.Collections.Generic;
//using Microsoft.Owin;
//using Hangfire.Dashboard;
//using AccountEx.Common;

//namespace AccountEx.Web.Filters
//{
//    public class HangfireAuthorizationFilter : Hangfire.Dashboard.IAuthorizationFilter
//    {
//        public bool Authorize(IDictionary<string, object> owinEnvironment)
//        {
//            // In case you need an OWIN context, use the next line,
//            // `OwinContext` class is the part of the `Microsoft.Owin` package.
//            var context = new OwinContext(owinEnvironment);

//            // Allow all authenticated users to see the Dashboard (potentially dangerous).
//            // return context.Authentication.User.Identity.IsAuthenticated;
//            return true;
//            //if (SiteContext.Current.User != null && SiteContext.Current.User.IsAdmin)
//            //    return true;
//            //else
//            //    return false;
//        }






//    }
//}
