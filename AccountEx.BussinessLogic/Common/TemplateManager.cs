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
using System.Data;
using System.Dynamic;
using Scriban.Runtime;
using Scriban;

namespace AccountEx.BussinessLogic
{


    public static class TemplateManager
    {

        public static string ReplaceSalesTag(int saleId)
        {
            var salesTagTypes = new List<byte>() { (byte)TagType.Sale, (byte)TagType.SaleItem };
            var tagRepo = new TemplateTagRepository();
            var sqlRepo = new SqlRepository();
            var templateRepo = new TemplateRepository();
            var sale = new ExpandoObject() as IDictionary<string, Object>;
            var saleItems = new List<dynamic>();


            var template = templateRepo.FirstOrDefault();



            var tags = tagRepo.AsQueryable().Where(p => salesTagTypes.Contains(p.TagType)).Select(p =>
            new
            {
                p.TagType,
                p.ColumnName,
                TableName = p.TableName
            }).ToList();
            var salesTag = tags.Where(p => p.TagType == (byte)TagType.Sale).ToList();
            var columns = string.Join(",", salesTag.Select(p => p.ColumnName).ToList());
            if (!string.IsNullOrWhiteSpace(columns))
            {
                var tableName = salesTag.FirstOrDefault().TableName;
                var query = string.Format("Select {0} From dbo.{1} Where Id={2} AND CompanyId={3} And FiscalId={4}", columns, tableName, saleId, SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
                var dt = sqlRepo.GetDataTable(query);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    foreach (var tag in salesTag)
                    {
                        sale.Add(tag.ColumnName, row[tag.ColumnName]);

                    }
                }


            }
            var salesItemTag = tags.Where(p => p.TagType == (byte)TagType.SaleItem).ToList();
            columns = string.Join(",", salesItemTag.Select(p => p.ColumnName).ToList());
            if (!string.IsNullOrWhiteSpace(columns))
            {
                var tableName = salesItemTag.FirstOrDefault().TableName;
                var query = string.Format("Select {0} From dbo.{1} Where SaleId={2} AND CompanyId={3}", columns, tableName, saleId, SiteContext.Current.User.CompanyId);
                var dt = sqlRepo.GetDataTable(query);
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        var saleItem = new ExpandoObject() as IDictionary<string, Object>;
                        foreach (var tag in salesItemTag)
                        {
                            saleItem.Add(tag.ColumnName, row[tag.ColumnName]);
                        }
                        saleItems.Add(saleItem);


                    }
                }


            }


            var model = new
            {
                Sale = sale,
                SaleItems = saleItems
            };




            var context = GetScibanContext(model);
            var scrTemplate = Scriban.Template.Parse(template.Contents);
            //var html = scrTemplate.Render(model, member => member.Name); // => "Hello World!" 
            var html = scrTemplate.Render(context); // => "Hello World!" 
            return html;


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

        private static TemplateContext GetScibanContext(object model)
        {
            var script = new ScriptObject();
            script.Import(typeof(Numerics), renamer: member => member.Name);
            script.Import(typeof(UtilityFunctionManager), renamer: member => member.Name);
            script.Import(model, renamer: member => member.Name);
            var context = new TemplateContext() { MemberRenamer = member => member.Name };
            context.PushGlobal(script);
            return context;
        }
    }
}
