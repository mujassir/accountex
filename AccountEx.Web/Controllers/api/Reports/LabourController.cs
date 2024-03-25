using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace AccountEx.Web.Controllers.api.Reports
{
    public class LabourController : BaseApiController
    {
        public JQueryResponse Get()
        {
            var type = (QueryString["type"] + "").Trim();
            return GetDataTableForReport();
        }
        protected JQueryResponse GetDataTableForReport()
        {
            var queryString = QueryString;
            var coloumns = new[] { "Id", "Code", "Name", "PurchasePrice", "SalePrice", "Manufacturer", "Others", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type = (queryString["type"] + "").Trim();
            DateTime date1, date2;

            var vType = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));
            var repo = new SaleRepository();
            var records = repo.AsQueryable().Where(p => p.TransactionType == vType);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.AccountName.Contains(search)
                    || p.AccountCode.Contains(search)
                    );
            if (DateTime.TryParse(queryString["date1"], out date1))
                filteredList = filteredList.Where(p => p.Date >= date1);
            if (DateTime.TryParse(queryString["date2"], out date2))
                filteredList = filteredList.Where(p => p.Date <= date2);
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add((++sr) + "");
                data.Add("<a href='../Transaction/MapAction?type=" + item.TransactionType + "&voucherno=" + item.VoucherNumber + "'>" + item.VoucherNumber + "</a>");
                data.Add(item.InvoiceNumber + "");
                data.Add(item.Date.ToString(AppSetting.DateFormat));
                data.Add(item.AccountCode);
                data.Add(item.AccountName + "");
                data.Add(Numerics.DecimalToString(item.GrossTotal, 0));
                data.Add(Numerics.DecimalToString(item.Discount, 0));
                data.Add(Numerics.DecimalToString(item.NetTotal, 0));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
