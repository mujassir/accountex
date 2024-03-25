using System.Collections.Generic;
using System.Web.Mvc;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using AccountEx.Web.Code;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Repositories.COA;
using Entities.CodeFirst;
using AccountEx.Repositories.Config;
using System.Transactions;

namespace AccountEx.Web.Controllers.mvc
{
    [Compress]
    public class PrintController : BaseController
    {

        public ActionResult SaleInvoice(int Id)
        {
            ViewBag.Html = TemplateManager.ReplaceSalesTag(Id);
            return View("~/Views/Print/PrintContainer.cshtml");
        }
    }
}
