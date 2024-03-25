using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace AccountEx.Web.Handlers
{
    /// <summary>
    /// Summary description for SessionHeartbeatHttpHandler
    /// </summary>
    public class SessionHeartbeatHttpHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Session["Heartbeat"] = DateTime.Now;
            //context.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            //context.Response.Cache.SetValidUntilExpires(false);
            //context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //context.Response.Cache.SetNoStore();
            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}