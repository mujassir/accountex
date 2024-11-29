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
    public class DeliveryChallanController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        public virtual ApiResponse Get(string key, string dcIds)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (byte)((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));
                var Ids = dcIds.Split(',').Select(p => Numerics.GetInt(p)).ToList();
                var data = new DeliveryChallanRepository().AsQueryable(true).Where(p => Ids.Contains(p.Id)).SelectMany(p => p.DCItems).GroupBy(q => new { q.ItemId, q.ItemCode, q.ItemName }).Select(r => new
                {

                    r.Key.ItemId,
                    r.Key.ItemCode,
                    r.Key.ItemName,
                    r.FirstOrDefault().UnitType,
                    r.FirstOrDefault().Rate,
                    Amount = r.Sum(m => m.Amount),
                    NetAmount = r.Sum(m => m.NetAmount),
                    Quantity = r.Sum(m => m.Quantity),


                }).ToList();


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
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var repo = new DeliveryChallanRepository();
                var orderbookingrepo = new OrderBookingRepository(repo);
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }

                var vouchertype = (VoucherType)Convert.ToByte(type);
                Object orderinfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = repo.GetNextVoucherNumber(vouchertype, locationId);
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, key, locationId, out next, out previous);
                var detail = new AccountDetail();
                var salesmanInfo = "";
                var productMappings = new List<ProductMapping>();

                if (data != null)
                {
                    orderinfo = orderbookingrepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p => new
                    {
                        p.Date,
                        p.DeliveryDate,
                    }
                    ).FirstOrDefault();
                    if (data.AccountId != null)
                        detail = new AccountDetailRepository().GetByAccountId(data.AccountId.Value);
                    if (data.SalesmanId != null && data.SalesmanId != 0)
                    {
                        var saleMan = new AccountDetailRepository().AsQueryable().FirstOrDefault(p => p.AccountId == data.SalesmanId);
                        if (saleMan != null)
                            salesmanInfo = saleMan.ContactNumber;
                    }

                    productMappings = new ProductMappingRepository().AsQueryable().Where(p => p.CustomerId == data.AccountId).ToList();

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
                        AccountDetail = detail,
                        salesmanInfo = salesmanInfo,
                        ProductMappings = productMappings,

                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]DeliveryChallan input)
        {
            ApiResponse response;
            try
            {
                var err = DCManager.ValidateSave(input);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
            var records = new GenericRepository<vw_PendingDeliveryChallan>().AsQueryable(true).Where(p => p.Status != (byte)TransactionStatus.Delivered && p.TransactionType == (byte)type);
            if (pageType > 0)
                records = records.Where(p => p.InvoiceTransactionType == pageType);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.OrderNumber == intSearch ||
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
                data.Add("<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' /><input type='text' class='DcId hide' value='" + item.Id + "' />" + item.VoucherNumber + "");
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
            if (type == VoucherType.GoodIssue)
            {
                var productionStatueses = new List<byte> { (byte)TransactionStatus.FGRN, (byte)TransactionStatus.Pending, (byte)TransactionStatus.PartialyDelivered };
                records = records.Where(p => p.TransactionType == VoucherType.SaleOrder
                    && ((p.OrderType == (byte)OrderType.Production && productionStatueses.Contains(p.Status))
                    || (p.OrderType == (byte)OrderType.FinishedGoods && (p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.PartialyDelivered))
                    ));
            }
            else
            {
                records = records.Where(p => p.TransactionType == VoucherType.PurchaseOrder && (p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.PartialyDelivered));
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
                var code = (item.VoucherCode != null ? item.VoucherCode + "-" : "") + item.VoucherNumber;
                var data = new List<string>();
                data.Add("<input type='text' class='OrderNo form-control hide' value='" + code + 
                    "' data-location-Id='" + item.AuthLocationId +"' data-location-code='" + item.VoucherCode +"'/>" + code + "");
                data.Add(item.AccountName);
                data.Add(item.SalesmanName);
                data.Add(item.PartyPONumber);
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
