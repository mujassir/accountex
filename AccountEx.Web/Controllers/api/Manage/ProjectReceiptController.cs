using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ProjectReceiptController : BaseApiController
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
                response = new ApiResponse
                {
                    Success = true,
                    Data = new ProjectReceiptRepository().GetById(id),

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]ProjectReceipt input)
        {
            ApiResponse response;
            try
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                response = new ProjectReceiptRepository().AddReceipt(input) ? new ApiResponse { Success = true, Data = input.Id } : new ApiResponse { Success = false, Error = "Voucher Number is not valid! Please enter a valid Voucher Number" };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;

        }



        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new ProjectReceiptRepository().DeleteByVoucher(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "VoucherNumber", "AccountTitle", "AccountTitle", "Comment", "Debit", "Date", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var projectId = Numerics.GetInt(queryString["ProjectId"]);
            var voucherIds = new ProjectReceiptRepository().GetVoucherIds(projectId);
            var dal = new VoucherTransRepository();
            //var records = dal.AsQueryable();
            var records = dal.AsQueryable(true);
            records = records.Where(p => voucherIds.Contains(p.Id));
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch
                    );
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
            foreach (var item in orderedList)
            {
                var party = item.VoucherItems.Any(p => p.Debit > 0) ? item.VoucherItems.FirstOrDefault(p => p.Debit > 0) : new VoucherItem();
                var data = new List<string>
                {
                   
                    item.VoucherNumber + "",
                    item.Date.ToString(AppSetting.DateFormat),
                   party.AccountCode+"-"+party.AccountName,
                   party.Description,
                   Numerics.DecimalToString(party.Debit, party.Credit),
                };
                //var editIcon = "<i class='fa fa-edit' onclick=\"Project.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"ProjectReceipts.Delete(" + item.VoucherNumber + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                //icons += editIcon;
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
