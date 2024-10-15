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

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class FGRNController : BaseApiController
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
                var repo = new FGRNRepository();
                var orderBookingRepo = new OrderBookingRepository(repo);

                var voucherNumber = id;
                //var result = "";
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                Object orderinfo = "";
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = repo.GetNextVoucherNumber(vouchertype);
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                if (data != null)
                {
                    orderinfo = orderBookingRepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p => new
                    {
                        p.Date,
                        p.DeliveryDate,
                    }
                    ).FirstOrDefault();
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        WP = data,
                        OrderInfo = orderinfo,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]DeliveryChallan input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    DCManager.SaveFGRN(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                // var vouchertype =(VoucherType)Convert.ToByte(type);
                var voucherNo = Numerics.GetInt(queryString["voucherNo"]);
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    new FGRNRepository().Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };

                }

                return response;
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "AccountCode", "AccountName", "OrderNo", "OrderDate", "Date" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = (VoucherType) Convert.ToByte(queryString["type"]);
            var search = (queryString["sSearch"] + "").Trim();
            var records = new DeliveryChallanRepository().GetPendingDeliveryChallan(type);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.AccountName.Contains(search) ||
                    p.AccountCode.Contains(search)

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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add("<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                data.Add(item.AccountCode);
                data.Add(item.AccountName);
                data.Add(item.OrderNumber + "");
                //data.Add(item.OrderDate.ToString(AppSetting.GridDateFormat));
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        private string ServerValidateSave(DeliveryChallan input)
        {
            var err = ",";
            try
            {
                var repo = new DeliveryChallanRepository();
                var wipRepo = new WorkInProgressRepository();
                var record = repo.GetByVoucherNumber(input.VoucherNumber, input.TransactionType, input.Id, 0);
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                foreach (var item in input.DCItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Date should be within current fiscal year.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var WINP = wipRepo.GetByVoucherNumber(input.WPNo);

                if (WINP == null)
                {
                    err += "Invalid WP no.,";
                }
                var FGRN = repo.GetByWPNo(input.WPNo, VoucherType.FGRN, input.Id);

                if (FGRN != null)
                {
                    err += "WP no is already used in FGRN (Voucher No:" + FGRN.VoucherNumber + ").,";
                }
                if (input.Id > 0)
                {
                    var dbFGRN = repo.GetById(input.Id);
                    if (dbFGRN.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change FGRN no.please use previous FGRN no.(" + dbFGRN.VoucherNumber + "),";
                    }
                    if (dbFGRN.WPNo != input.WPNo)
                    {
                        err += "can't change WP no.please use previous WP no.(" + dbFGRN.OrderNo + "),";
                    }
                    FGRN = repo.GetByOrderNumber(input.OrderNo, VoucherType.GoodReceive);
                    if (FGRN != null)
                    {
                        err += "FGRN no is used in Good Received Note and Can't be updated.";
                    }
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var repo = new DeliveryChallanRepository();
                var FGRN = repo.GetById(id);
                var GRN = repo.GetByOrderNumber(FGRN.OrderNo, VoucherType.GoodIssue);
                if (GRN != null)
                {
                    err += "FGRN no is used in Good Received Note and Can't be updated.";
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
