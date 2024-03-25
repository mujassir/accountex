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
    public class VoucherController : BaseApiController
    {
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
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));
                var data = !Numerics.GetBool(queryString["loadwithvoucherno"]) ? new TransactionRepository().GetById(id) : new TransactionRepository().GetByVoucherNumber(vouchertype, id);
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
        public ApiResponse Post([FromBody]VoucherExtra input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    //input.Items.ForEach(p => p.FiscalId = SiteContext.Current.Fiscal.Id);
                    var queryString = Request.RequestUri.ParseQueryString();
                    if (queryString["mode"] == "add")
                        new VoucherRepository().Save(input);
                    else
                        new VoucherRepository().Update(input);
                    response = new ApiResponse { Success = true, Data = "" };
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
            var coloumns = new[] { "VoucherNumber", "VoucherNumber", "AccountTitle", "AccountTitle", "Comment", "Debit", "Date", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var isClientside = Numerics.GetBool(queryString["IsClientSide"]);
            var dal = new VoucherRepository();
            var cutsomers = new AccountRepository().AsQueryable().Where(p => p.Level == AppSetting.AccountLevel).Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));

            records = records.Where(p => p.TransactionType == vouchertype && p.EntryType == (byte)EntryType.MasterDetail);

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p => p.VoucherNumber == intsearch || p.AccountId == intsearch);
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
            var voucherlist = orderedList.Skip(displayStart).Take(displayLength).ToList().Select(p => p.VoucherNumber);
            var childrecord = dal.AsQueryable().Where(p => p.TransactionType == vouchertype && p.EntryType == (byte)EntryType.Item && voucherlist.Contains(p.VoucherNumber));
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var child = childrecord.FirstOrDefault(p => p.VoucherNumber == item.VoucherNumber);
                var data = new List<string>();
                var cus = cutsomers.FirstOrDefault(p => p.Id == item.AccountId);
                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.VoucherNumber + "");
                data.Add(cus != null ? cus.Name : "");
                var accid = child != null ? child.AccountId : 0;
                cus = cutsomers.FirstOrDefault(p => p.Id == accid);
                data.Add(cus != null ? cus.Name : "");
                data.Add(Numerics.DecimalToString(item.Debit, item.Credit));
                data.Add(item.Comments);
                data.Add(item.Date.ToString("dd/MM/yyyy"));
                var subcats = "";
                var printIcon = "<i class='fa fa-print' onclick=\"Voucher.Edit(" + item.Id + ",true)\" title='Edit' ></i>";
                var editIcon = "<i class='fa fa-edit' onclick=\"Voucher.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Voucher.Delete(" + item.Id + ")\" title='Delete' ></i>";

                var icons = "<span class='action'>";
                icons += printIcon;
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

        private string ServerValidateSave(VoucherExtra input)
        {
            var err = ",";
            try
            {
                foreach (var item in input.Items.Where(p => p.AccountId == 0))
                {
                    err += "Account is not valid to process the request,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
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
