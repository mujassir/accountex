using System.Text;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Linq;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BussinessLogic
{


    public static class ExcelFileManager
    {
       public static void ListToExcelUsineEpPlus<T>(List<T> query, string targetPath)
        {
            var file=new FileInfo(targetPath);
            using (ExcelPackage pck = new ExcelPackage(file))
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Default");

                //get our column headings
                var t = typeof(T);
                var Headings = t.GetProperties();
                for (int i = 0; i < Headings.Count(); i++)
                {

                    ws.Cells[1, i + 1].Value = Headings[i].Name;
                }

                //populate our Data
                if (query.Count() > 0)
                {
                    ws.Cells["A2"].LoadFromCollection(query);
                }

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:BZ1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                pck.Save();
            }
        }

    }
}
