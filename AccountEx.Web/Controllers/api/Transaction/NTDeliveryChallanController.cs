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
    public class NTDeliveryChallanController : BaseApiController
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
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                var orderRepo = new OrderBookingRepository();
                var dcRepo = new DeliveryChallanRepository(orderRepo);
                var cpRepo = new CompanyPartnerRepository(orderRepo);
                var companyPartner = new CompanyPartner();
                Object orderinfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = dcRepo.GetNextVoucherNumber(vouchertype);
                var data = dcRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                if (data != null && data.CompanyPartnerId != null)
                    companyPartner = cpRepo.GetById(data.CompanyPartnerId.Value);

                if (data != null)
                {
                    orderinfo = orderRepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p => new
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
                        Order = data,
                        OrderInfo = orderinfo,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        CompanyPartner = companyPartner
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
                var err = DCManager.ValidateSave(input, true);
                if (err == "")
                {

                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    DCManager.Save(input, true);
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
                var err = DCManager.ValidateDelete(id);
                if (err == "")
                {
                    DCManager.Delete(id);
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
            var coloumns = new[] { "VoucherNumber", "AccountCode", "AccountName", "OrderNo", "Date" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var dcNumbers = new List<int>();
            var type = Convert.ToByte(queryString["type"]);
            var pageType = Convert.ToByte(queryString["pageType"]);


            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            if (!string.IsNullOrWhiteSpace(queryString["dcNumbers"]))
                dcNumbers = (queryString["dcNumbers"].Split(',')).ToList().Select(p => Numerics.GetInt(p)).ToList();
            //var records = new DeliveryChallanRepository().GetPendingDeliveryChallan(type);
            var records = new GenericRepository<vw_PendingDeliveryChallan>().AsQueryable(true).Where(p => p.Status != (byte)TransactionStatus.Delivered);
            if (pageType > 0)
                records = records.Where(p => p.InvoiceTransactionType == pageType);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.OrderNumber == intSearch ||
                   (intSearch > 0 && p.DCQuatityTotal == intSearch) ||
                    p.AccountName.Contains(search) ||
                    p.AccountCode.Contains(search)

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
            foreach (var item in orderedList)
            {
                var data = new List<string>();
                var strChecked = "";
                if (dcNumbers.Contains(item.VoucherNumber))
                    strChecked = "checked='checked'";
                data.Add("<td><input type='checkbox' " + strChecked + " class='checkboxes' value='" + (string.IsNullOrWhiteSpace(strChecked) ? "0" : "1") + "' /></td>");
                data.Add("<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' /><input type='text' class='CompanyPartnerId form-control hide' value='" + item.CompanyPartnerId + "' /><input type='text' class='DcId hide' value='" + item.Id + "' />" + item.VoucherNumber + "");
                data.Add("<input type='text' class='InvoiceNumber form-control hide' value='" + item.InvoiceNumber + "' />" + item.InvoiceNumber + "");
               
                data.Add(item.DCQuatityTotal + "");
                data.Add(item.AccountCode);
                data.Add(item.AccountName);
                data.Add(item.OrderNumber + "");
                //data.Add(item.OrderDate.ToString(AppSetting.GridDateFormat));
                data.Add(item.Date.ToString(AppSetting.GridDateFormat1));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
        public JQueryResponse GetDataTable(string orders)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "AccountName", "SalesmanName", "Date", "DeliveryDate" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = (VoucherType)Convert.ToByte(queryString["type"]);
            var key = Convert.ToByte(queryString["key"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new OrderBookingRepository().AsQueryable();
            if (type  == VoucherType.GoodIssue)
            {
                var productionStatueses = new List<byte> { (byte)TransactionStatus.FGRN, (byte)TransactionStatus.Pending, (byte)TransactionStatus.PartialyDelivered };
                records = records.Where(p => p.TransactionType  == VoucherType.SaleOrder
                    && ((p.OrderType == (byte)OrderType.Production && productionStatueses.Contains(p.Status))
                    || (p.OrderType == (byte)OrderType.FinishedGoods && (p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.PartialyDelivered))
                    ));
            }
            else
            {
                records = records.Where(p => p.TransactionType  == VoucherType.PurchaseOrder && (p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.PartialyDelivered));
            }
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.AccountName.Contains(search) ||
                    p.SalesmanName.Contains(search)

                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;

            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList)
            {
                var data = new List<string>();
                data.Add("<input type='text' class='OrderNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                data.Add(item.AccountName);
                if (type == VoucherType.GoodIssue)
                    data.Add(item.SalesmanName);
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add((item.DeliveryDate.HasValue ? item.DeliveryDate.Value.ToString(AppSetting.GridDateFormat) : ""));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
