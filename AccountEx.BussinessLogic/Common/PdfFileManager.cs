using System.Text;
using System.Net.Mail;
using System.IO;
using SelectPdf;
using System;
using System.Web;

namespace AccountEx.BussinessLogic
{


    public static class PdfFileManager
    {

        public static string ConvertToPDF(string htmlString, string fileName, string path, string baseUrl)
        {
            
           
            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);
            HtmlToPdf converter = new HtmlToPdf();
            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            //converter.Options.WebPageWidth = webPageWidth;
            //converter.Options.WebPageHeight = webPageHeight;
            converter.Options.InternalLinksEnabled = false;
            converter.Options.ExternalLinksEnabled = false;
            converter.Options.MarginTop = 0;
            //converter.Options.MarginLeft = 5;
            //converter.Options.MarginRight = 5;
            converter.Options.MarginBottom = 0;
            // converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            doc.DocumentInformation.CreationDate = DateTime.Now;
            doc.DocumentInformation.Title = fileName;
            doc.DocumentInformation.Author = "";
            doc.DocumentInformation.Subject = fileName;

            // doc.Margins = new PdfMargins(10, 10, 0, 0);
            // create a new pdf font
            //PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
            //font.Size = 100;
            var dirPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var filename = fileName;
            var fullFilePath = dirPath + filename + ".pdf";
            doc.Save(fullFilePath);
            MemoryStream stream = new MemoryStream(doc.Save());
            // close pdf document
            doc.Close();
            return fullFilePath;
        }
    }
}
