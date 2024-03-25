using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class DiscountTransactionController : TransactionController
    {
        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var result = "";
                var data = new VwTransactionRepository().GetById(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "VoucherNumber", "InvoiceNumber", "", "Debit", "Date", "Comments", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var isClientside = Numerics.GetBool(queryString["IsClientSide"]);
            var dal = new TransactionRepository();
            var cutsomer = new AccountRepository().AsQueryable().Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));

            records = records.Where(p => p.TransactionType == vouchertype && p.EntryType == (byte)EntryType.MasterDetail);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p => p.InvoiceNumber == intsearch || p.VoucherNumber == intsearch || p.AccountId == intsearch);
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.VoucherNumber + "");
                data.Add(item.InvoiceNumber + "");
                var cus = cutsomer.FirstOrDefault(p => p.Id == item.AccountId);
                // data.Add(item.VoucherNumber + "");
                data.Add(cus != null ? cus.Name : "");
                var v = item.Debit + item.Credit;
                data.Add(Numerics.DecimalToString(item.Debit, item.Credit));
                data.Add(item.Date.ToString(AppSetting.DateFormat));
                data.Add(item.Comments);
                var editIcon = "<i class='fa fa-edit' onclick=\"DiscountTransaction.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"DiscountTransaction.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var invoiceIcon = "";

                invoiceIcon = "<a target='_blank' href='../Reports/Invoice?voucher-number=" + item.VoucherNumber + "&type=" + queryString["type"] + "'><i class='fa fa-book' title='Invoice' ></i></a>";
                var icons = "<span class='action'>";

                icons += invoiceIcon;
                icons += editIcon;
                icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
