using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System.Data.Entity.Validation;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class WorkInProcessController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();

                var key = queryString["key"].ToLower();
                var type = queryString["type"];
                var entrytype = Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransactionRepository().GetNextVoucherNumber(VoucherType.Production);
                var data = new WorkInProgressRepository().GetByVoucherNumber(voucherNumber, (byte)VoucherType.Production, key, out next, out previous);
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
                response = new ApiResponse { Success = false, Error = ex.Message };Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return response;
        }

        public ApiResponse Post([FromBody]WorkInProgress input)
        {
            ApiResponse response;
            try
            {

                ProductionManager.Save(input,true);

                response = new ApiResponse { Success = true, Data = input.Id };
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //catch (Exception ex)
            //{
            //    response = new ApiResponse { Success = false, Error = ex.Message };Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            //}

            return response;

        }
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] {"WPNo","OrderDate"};
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = Convert.ToByte(queryString["type"]);
            var search = (queryString["sSearch"] + "").Trim();
            //var records = new WPItemsRepository().AsQueryable().Where(p => p.EntryType == type);
            var records = new WorkInProgressRepository().AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.AccountName.Contains(search)
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add("<input type='text' class='VoucherNumber form-control' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                //data.Add(item.VoucherNumber.ToString());
                //data.Add(item.OrderNo.ToString());
                //data.Add((item.Date.ToString(AppSetting.GridDateFormat)));
                data.Add((item.OrderDate.HasValue ? item.OrderDate.Value.ToString(AppSetting.GridDateFormat) : ""));
                //data.Add((item.DeliveryDate.HasValue ? item.DeliveryDate.Value.ToString(AppSetting.GridDateFormat) : ""));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                ProductionManager.Delete(id, (byte)VoucherType.Production);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ex.Message };Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return response;
        }
    }
}
