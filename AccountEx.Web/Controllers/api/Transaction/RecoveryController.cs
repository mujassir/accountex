using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.DbMapping;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
// ReSharper disable All

namespace AccountEx.Web.Controllers.api
{
    public class RecoveryController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return GetRecoveryData();
        }


        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var result = "";
                Request.RequestUri.ParseQueryString();
                var data = new TransactionRepository().GetById(id);
                var dictionary = new Dictionary<string, bool>();

                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

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

                new RecoveryRepository().Save(input);
                response = new ApiResponse { Success = true, Data = "" };


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
                new AccountRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Name", "NoOfQuestions", "TotalTime", "" };
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
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"]));

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

                data.Add(item.VoucherNumber + "");
                data.Add(item.InvoiceNumber + "");
                var cus = cutsomer.FirstOrDefault(p => p.Id == item.AccountId);
                data.Add(item.VoucherNumber + "");
                data.Add(cus != null ? cus.Name : "");

                data.Add(item.Debit + "");
                data.Add(item.Date.ToString("dd/MM/yyyy"));
                var subcats = "";
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
                var editIcon = "<i class='icon-edit' onclick=\"Edit(" + item.Id + ")\" data-original-title='Edit' ></i>";
                var deleteIcon = "<i class='icon-remove' onclick=\"Delete(" + item.Id + ")\" data-original-title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                // icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        private ApiResponse GetRecoveryData()
        {
            ApiResponse response;
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var queryString = Request.RequestUri.ParseQueryString();
            var date1 = queryString["date"];
            var ci = new CultureInfo("en-US");
            var date = Convert.ToDateTime(queryString["date"] + "", ci);

            var customer = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var rep = new AccountRepository();
            var account = rep.GetLeafAccount(customer);


            var list = new List<RecoveryExtra>();
            var data = new RecoveryExtra();
            //  var balances = new TransactionRepository().GetOpeningStartBalance(account.Select(p => p.Id).ToList());
            foreach (var item in account)
            {
                data = new RecoveryExtra();
                data.PreviousBalance = new TransactionRepository().GetOpeningBalance(item.Id, date);
                data.AccountId = item.Id;
                data.AccountTitle = item.Name;
                data.TodaySale = Numerics.GetDouble(new TransactionRepository().GetTodaySale(item.Id, date));
                data.TodayBalance = Numerics.GetDouble(data.PreviousBalance) + data.TodaySale;
                var recovery = new TransactionRepository().GetRecovery(item.Id, date);
                if (recovery != null)
                {
                    data.Received = Numerics.GetInt(recovery.Credit);
                    data.Id = recovery.Id;
                }
                data.NetBalance = Numerics.GetDouble(data.TodayBalance) - data.Received;
                list.Add(data);

            }
            response = new ApiResponse { Success = true, Data = list };
            return response;
        }
    }
}
