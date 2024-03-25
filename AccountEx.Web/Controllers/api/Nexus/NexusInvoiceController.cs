using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.Repositories;
using AccountEx.Repositories.Config;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.BussinessLogic.CRM;
using AccountEx.CodeFirst.Models.Nexus;
using System.IO;
using SelectPdf;
using System.Web;
using BussinessLogic;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class NexusInvoiceController : BaseApiController
    {
        public ApiResponse Get()
        {


            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new NexusCaseRepository();
            var transactions = repo.GetInvoicePrintingData(accountId, date1, date2);
            var groupData = transactions.GroupBy(p => p.CaseNumber).Select(p => new
            {
                CaseNumber = p.Key,
                Cases = p.ToList()
            });
            var files = new List<string>();
            var departmentName = "";
            foreach (var postedCase in groupData)
            {

                var parentInfo = postedCase.Cases.FirstOrDefault();
                departmentName = parentInfo.Department;
                string printTemplate = "~/Views/Nexus/invoice.html";
                string baseUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                // var htmlString = UtilityFucntionManager.ReplacePlaceHolder("~/Views/PrintTemplate/FeeSlip.html", feechallan);
                var htmlString = UtilityFunctionManager.ReplacePlaceHolder(printTemplate, parentInfo);
                var testHtml = "";
                var sn = 1;
                var total = 0M;
                foreach (var item in postedCase.Cases)
                {
                    testHtml += "<tr>";
                    testHtml += "<td class='align-center'>" + sn + "</td>";
                    testHtml += "<td class='align-left'> " + item.TestName + "</td>";
                    testHtml += "<td class='align-right'>" + Numerics.IntToString(item.Price) + "</td>";
                    testHtml += " </tr>";
                    total += item.Price;
                    sn++;
                }



                htmlString = htmlString.Replace("{baseUrl}", baseUrl);
                htmlString = htmlString.Replace("{TestHtml}", testHtml);
                htmlString = htmlString.Replace("{TotalAmount}", Numerics.IntToString(total));
                htmlString = htmlString.Replace("{AmountInWord}", UtilityFunctionManager.NumberToWords(Numerics.GetInt(total)));
                htmlString = htmlString.Replace("{Date1}", parentInfo.Date.ToString("dd-MM-yyyy"));
                htmlString = htmlString.Replace("{Date2}", parentInfo.Date.ToString("MMM-yy"));
                htmlString = htmlString.Replace("{UserName}", SiteContext.Current.User.Username);
                htmlString = htmlString.Replace("{ReportHeader}", HttpUtility.HtmlDecode(SettingManager.ReportHeader));



                string pdf_page_size = "A4";
                PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                    pdf_page_size, true);

                string pdf_orientation = "Portrait";
                PdfPageOrientation pdfOrientation =
                    (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;
                HtmlToPdf converter = new HtmlToPdf();
                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                //converter.Options.WebPageWidth = webPageWidth;
                //converter.Options.WebPageHeight = webPageHeight;
                converter.Options.InternalLinksEnabled = false;
                converter.Options.ExternalLinksEnabled = false;
                converter.Options.MarginTop = 0;
                converter.Options.MarginLeft = 20;
                converter.Options.MarginRight = 20;
                converter.Options.MarginBottom = 0;
                // converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
                doc.DocumentInformation.CreationDate = DateTime.Now;
                doc.DocumentInformation.Title = parentInfo.Department;
                doc.DocumentInformation.Author = SettingManager.ApplicationTitle;
                doc.DocumentInformation.Subject = parentInfo.Department;

                // doc.Margins = new PdfMargins(10, 10, 0, 0);
                // create a new pdf font
                //PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
                //font.Size = 100;
                var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                var filename = parentInfo.CaseNumber.Replace("-", "_");
                var fullFilePath = dirPath + filename + ".pdf";
                doc.Save(fullFilePath);
                MemoryStream stream = new MemoryStream(doc.Save());
                // close pdf document
                doc.Close();
                files.Add(fullFilePath);

            }
            var dirPath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");
            if (!Directory.Exists(dirPath1)) Directory.CreateDirectory(dirPath1);
            var filename1 = "Posted_Cases_" + UtilityFunctionManager.CleanFileName(departmentName) + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".pdf";
            var fullFilePath1 = dirPath1 + filename1;
            var response = new ApiResponse();
            if (files.Count > 0)
            {
                PDFMerge.MergePDFs(files, fullFilePath1);

                response = new ApiResponse
                {
                    Success = true,
                    Data = "../Upload/Temp/" + filename1
                };
            }
            else
            {
                response = new ApiResponse
                {
                    Success = false,
                    Error = "No Invoice Found for Printing."
                };
            }

            return response;


        }



    }
}
