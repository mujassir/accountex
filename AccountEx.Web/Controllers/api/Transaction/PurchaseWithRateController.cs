using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class PurchaseWithRateController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
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

        // POST api/test
        public ApiResponse Post([FromBody]SaleExtra input)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    if (queryString["mode"] == "add")
                        new TransactionRepository().Save(input);
                    else
                        new TransactionRepository().Update(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new TransactionRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        protected virtual JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "VoucherNumber", "InvoiceNumber", "", "Debit", "Date", "Comments", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());


            var records = new VwTransactionRepository().AsQueryable();
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
            var data12 = orderedList.ToList();
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.VoucherNumber + "");
                data.Add(item.InvoiceNumber + "");
                data.Add(item.Code + "-" + item.Account);
                var v = item.Debit + item.Credit;
                data.Add(Numerics.DecimalToString(item.Debit, item.Credit));
                data.Add(item.Date.ToString(AppSetting.DateFormat));
                data.Add(item.Comments);
                var editIcon = "<i class='fa fa-edit' onclick=\"PurchaseWithRate.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"PurchaseWithRate.Delete(" + item.Id + ")\" title='Delete' ></i>";
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

        private string ServerValidateSave(SaleExtra input)
        {
            var err = ",";
            try
            {
                foreach (var item in input.Items.Where(p => p.AccountId == 0))
                {
                    err += "Account is not valid to process the request,";
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
    }
}
