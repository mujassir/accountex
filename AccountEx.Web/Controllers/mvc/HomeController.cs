using System;
using System.IO;
using System.Text;
using System.Web.Mvc;


using AccountEx.Common;
using AccountEx.BussinessLogic;
using AccountEx.Repositories;
using AccountEx.Web.Code;
using AccountEx.CodeFirst.Models;
using SelectPdf;
using AccountEx.BussinessLogic.Security;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccountEx.Web.Controllers.mvc
{
    //[Compress]
    public class HomeController : BaseController
    {
        public ActionResult XDashboard()
        {

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "DashBaordId", Value = DashboardManager.GetDashBoardIdByRoleIds() });
            setting.Add(new SettingExtra() { Key = "UserId", Value = SiteContext.Current.User.Id });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }

        public ActionResult Index()
        {

            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            // MakeReport();
            // return RedirectToAction("index", "transaction", new { type = "Sale" });
            if (SiteContext.Current.User == null)
                //if (!Response.IsRequestBeingRedirected)
                return RedirectToAction("login", "Account");
            else
            {
                var url = SettingManager.Get("Other.DashBoardUrl");
                if (string.IsNullOrWhiteSpace(url))
                    url = "~/home/xdashboard?name=AdminDashboard";

                if (!string.IsNullOrWhiteSpace(Request["ReturnURL"]))
                {
                    if (!Response.IsRequestBeingRedirected)
                        Response.Redirect(Request["ReturnURL"]);
                }
                else
                {
                    if (!Response.IsRequestBeingRedirected)
                        Response.Redirect(Url.Content(url.Replace("..", "~")));
                }



            }
            return View();


        }

        public ActionResult AboutUs()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        //public ActionResult sitemaintenance()
        //{

        //    return View();
        //}
        //[PartialCache Attribute("Long")]
        public ActionResult Menu()
        {
            return PartialView("_Menu");
        }


        public ActionResult xproductversions()
        {
            ViewBag.ProductVersions = new GenericRepository<ProductVersion>().GetAll();
            return View();
        }


    }
}
