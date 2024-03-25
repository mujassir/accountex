using LowercaseDashedRouting;
using System.Web.Mvc;
using System.Web.Routing;

namespace AccountEx.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
          //  routes.MapPageRoute("HtmlRoute", "sitemaintenance", "~/home/sitemaintenance.html");
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            routes.Add(new LowercaseDashedRoute("{controller}/{action}/{id}",
                    new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        id = UrlParameter.Optional
                    }), new DashedRouteHandler()));

           
            //routes.MapPageRoute(
        }
    }
}