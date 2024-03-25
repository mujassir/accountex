using System.Text;
using System.Net.Mail;
using System.IO;
using AccountEx.Common;
using AccountEx.BussinessLogic.Security;
using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;
using AccountEx.Repositories;
using System.Linq;
using System.Web;

namespace AccountEx.BussinessLogic
{


    public static class BarCodeManager
    {

        public static string PrintBarcode(List<IdsExtra> accountIds, string baseUrl)
        {

            string template = "~/Views/PrintTemplate/Barcode.html";
            var Ids = accountIds.Select(p => p.Id).ToList();
            var records = new AccountDetailRepository().AsQueryable().Where(p => Ids.Contains(p.Id) && p.AccountDetailFormId == (byte)AccountDetailFormType.Products).Select(p => new
            {
                p.BarCode,
                p.Name,
                p.Code
            });
            var html = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(template));
            var barcodeHtml = "";
            foreach (var product in records)
            {
                var url = baseUrl + "upload/temp/barcode/" + product.BarCode + ".gif";
                CreateBarCode(product.BarCode, "~/upload/temp/barcode/");
                barcodeHtml += "<p class='barcode-container'>";
                barcodeHtml += "<img src='" + url + "' alt='Barcode of " + product.Code + "' />";
                //barcodeHtml += "<span class='barcode-value'>" + product.BarCode + " (" + product.Code + ")" + "</span>";
                //barcodeHtml += "<span class='barcode-value'>" + product.Name + "</span>";
                barcodeHtml += "</p>";

            }
            html = html.Replace("{barcodeHtml}", barcodeHtml);
            var fileName = new Random().Next() + "_barcode";
            PdfFileManager.ConvertToPDF(html, fileName, "~/Upload/Temp/", baseUrl);
            return "../../upload/temp/" + fileName + ".pdf";
        }

        private static void CreateBarCode(string barCode, string path)
        {

            // mrno = mrno.Replace("/", "");
            barCode = barCode.Trim();
            var dirPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            string fullPath = dirPath + barCode + ".gif";
            //barCode = "*" + barCode + "*";
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
            var b = new BarcodeLib.Barcode();
            b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
            var type = (BarcodeLib.TYPE)Enum.Parse(typeof(BarcodeLib.TYPE), SettingManager.BarcodeType, true); //BarcodeLib.TYPE.CODE39;

            try
            {
                b.RotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                b.IncludeLabel = true;
                b.AlternateLabel = barCode;
                System.Drawing.Image img = b.Encode(type, "*" + barCode + "*", System.Drawing.Color.Black, System.Drawing.Color.White, 
                    SettingManager.BarcodeWidth, SettingManager.BarcodeHeight);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                img.Save(fullPath);
            }
            catch (Exception ex)
            {

                var data = ex.Data.ToString();
            }
        }
    }
}
