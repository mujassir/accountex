using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api
{
    public class GstTransactionController : ApiController
    {

        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);
                var data = new SaleRepository().GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public ApiResponse Get(int editid, string type)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = editid;
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));
                var data = !Numerics.GetBool(queryString["loadwithvoucherno"]) ? new SaleRepository().GetById(editid) : new SaleRepository().GetByVoucherNumber(voucherNumber, vouchertype);
                response = new ApiResponse
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Post([FromBody]Sale input)
        {
            ApiResponse response;
            try
            {
                var err = TransactionManager.ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    GstTransactionManager.Save(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;

        }

        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                var vouchertype =(VoucherType)Convert.ToByte(type);
                GstTransactionManager.Delete(id,vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public JQueryResponse GetDataTable()
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
            var dal = new SaleRepository();
            var cutsomer = new AccountRepository().AsQueryable().Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));

            records = records.Where(p => p.TransactionType == vouchertype);
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

                data.Add(item.AccountCode + "-" + item.AccountName);

                data.Add(Numerics.DecimalToString(item.NetTotal));
                data.Add(item.Date.ToString(AppSetting.DateFormat));
                data.Add(item.Comments);
                var editIcon = "<i class='fa fa-edit' onclick=\"GstTransaction.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"GstTransaction.Delete(" + item.VoucherNumber + ")\" title='Delete' ></i>";
                var invoiceIcon = "";

                invoiceIcon = "<a target='_blank' href='../Reports/GSTInvoice?voucher-number=" + item.VoucherNumber + "&type=" + queryString["type"] + "'><i class='fa fa-book' title='Invoice' ></i></a>";
                var icons = "<span class='action'>";
                //if (Vouchertype == VoucherType.Sale || (Vouchertype == VoucherType.SaleReturn))
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
