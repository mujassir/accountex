using System;
//using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
//using Hangfire;
using BussinessLogic;
using AccountEx.Web.Filters;
using AccountEx.BussinessLogic;

[assembly: OwinStartup(typeof(AccountEx.Web.Startup))]
namespace AccountEx.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    AuthorizationFilters = new[] { new HangfireAuthorizationFilter() }
            //});



//#if !DEBUG
//           app.UseHangfireServer();
//           RecurringJob.AddOrUpdate("rudolf_calendar_events_sync_jobs", () => GoogleCalendarManager.SyncRudolfCalendarEventUsingHangfire(), "*/20 * * * *", TimeZoneInfo.Local);
//#else

//#endif


//           // RecurringJob.AddOrUpdate("rudolf_calendar_events_sync_jobs", () => GoogleCalendarManager.SyncRudolfCalendarEventUsingHangfire(), "*/20 * * * *", TimeZoneInfo.Local);



        }
    }
}
