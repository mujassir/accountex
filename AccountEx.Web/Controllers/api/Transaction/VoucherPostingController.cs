using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api
{
    public class VoucherPostingController : BaseApiController
    {

        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {

                var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
                var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
                var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
                var voucherslist = !string.IsNullOrWhiteSpace(Request.GetQueryString("vouchertype"))
                    ? Request.GetQueryString("vouchertype").Split(',').ToList()
                    : null;
                var vouchers = new List<VoucherType>() { VoucherType.PurchaseReturn, VoucherType.CashReceipts, VoucherType.CashPayments, VoucherType.BankReceipts, VoucherType.BankPayments, VoucherType.TransferVoucher };
                if (voucherslist != null) vouchers = voucherslist.Select(p => (VoucherType)Numerics.GetByte(p)).ToList();
                var data = ReportManager.GetUnpostedVoucherList(date1, date2, vouchers).OrderBy(p => p.Date).ToList();
                var userIds = data.Select(p => p.CreatedBy).Distinct().ToList();
                var users = new UserRepository().GetIdNames(userIds);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Vouchers = data,
                        Users = users
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]List<VoucherPostingExtra> records)
        {
            ApiResponse response;
            try
            {

                VoucherManager.PostBulkVoucher(records);
                response = new ApiResponse { Success = true, Data = null };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ex.Message + " " + ex.InnerException };
            }

            return response;

        }





        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "VoucherNumber", "AccountTitle", "Debit", "Date", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var isClientside = Numerics.GetBool(queryString["IsClientSide"]);
            var dal = new SaleRepository();
            var cutsomers = new AccountRepository().AsQueryable().Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();
            // var Vouchertype = (int)((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"]));



            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p => p.InvoiceNumber == intsearch || p.VoucherNumber == intsearch || p.AccountId == intsearch);
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
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
            var ci = new CultureInfo("en-US");
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string> { item.VoucherNumber + "", item.InvoiceNumber + "" };


                var cus = cutsomers.FirstOrDefault(p => p.Id == item.AccountId);
                data.Add(cus != null ? cus.Name : "");


                data.Add(item.Date == DateTime.MinValue ? item.Date.ToString("dd-MMM-yyyy", ci) : "");
                //foreach (var subcat in item.BranchCategories.ToList())
                //{
                //    subcats += subcat.Category != null ? "<label class='label label-info'>" + subcat.Category.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(subcats);
                //var halls = "";
                //foreach (var hall in item.BranchHalls.ToList())
                //{
                //    var dbhal = halldata.FirstOrDefault(p => p.Id == hall.HallId);
                //    halls += dbhal != null ? "<label class='label label-important'>" + dbhal.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(halls);

                //data.Add(item.NoOfQuestions + "");
                //data.Add(item.TotalTime + "");
                var deleteIcon = "<i class='icon-remove' onclick=\"Delete(" + item.Id + ")\" data-original-title='Delete' ></i>";
                var printIcon = "<i class=' icon-print' onclick=\"PrintFunction(" + item.Id + ")\" data-original-title='Print' ></i>";



                var icons = "<span class='action'>";
                // icons += editIcon;
                icons += printIcon;
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
