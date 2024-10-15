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
    public class OrderBookingController : BaseApiController
    {
        public JQueryResponse Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var type = (queryString["type"] + "").Trim();
            var key = (VoucherType)Convert.ToByte(queryString["key"]);
            if (key == VoucherType.Services)
                return GetDataTableForServices();
            else
                return GetDataTable();
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var currency = new object();
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();

                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }

                var bookNo = Numerics.GetInt(queryString["bookNo"]);
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var orderbookingRepo = new OrderBookingRepository();
                var currRepo = new CurrencyRepository(orderbookingRepo);
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = orderbookingRepo.GetNextVoucherNumber(vouchertype, locationId);

                if (key == "byorderno")
                {
                    key = "same";
                    voucherNumber = orderbookingRepo.GetVoucherNoByBookNumber(bookNo, vouchertype, locationId);
                }

                var data = orderbookingRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, locationId, out next, out previous);
                if (data != null)
                    currency = currRepo.GetCurrencyById(data.CurrencyId);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        Currency = currency,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Get(int accountId, string key)
        {
            ApiResponse response;
            try
            {
                var record = new AccountDetailRepository().AsQueryable().Where(p => p.AccountId == accountId).Select(q => new
                {
                    q.Email,
                    q.ContactPerson,

                }).FirstOrDefault();

                response = new ApiResponse
                {
                    Success = true,
                    Data = record
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]Order input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    OrderBookingManager.Save(input);
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
                var type = (queryString["type"]);
                var vouchertype = (VoucherType)Convert.ToByte(type);
                var voucherNo = Numerics.GetInt(queryString["voucherNo"]);
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    OrderBookingManager.Delete(id);
                    response = new ApiResponse { Success = true };
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
        private string ServerValidateSave(Order input)
        {
            var err = ",";
            try
            {
                var orderbookingRepo = new OrderBookingRepository();
                var dcRepo = new DeliveryChallanRepository(orderbookingRepo);
                var jobORRepo = new JobOrderRequisitionRepository(orderbookingRepo);

                var goodsNoteType = input.TransactionType == VoucherType.SaleOrder ? VoucherType.GoodIssue : VoucherType.GoodReceive;
                var record = orderbookingRepo.GetByVoucherNumber(input.VoucherNumber, input.TransactionType, input.Id, input.AuthLocationId);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new voucher.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update voucher.,";
                        }
                    }
                }

                if (input.AccountId == 0)
                {
                    err += "Account is not valid to process the request.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                foreach (var item in input.OrderItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Date should be within current fiscal year.,";
                }
                record = orderbookingRepo.GetByBookNumber(input.InvoiceNumber, input.TransactionType, input.Id, input.AuthLocationId);

                if (record != null)
                {
                    err += "Book no already exist.,";
                }
                record = orderbookingRepo.GetBySRN(input.VoucherNumber, VoucherType.SaleOrder, input.Id, input.AuthLocationId);
                if (input.TransactionType == VoucherType.PurchaseOrder)
                {
                    if (record != null)
                    {
                        err += "SRN no is already used.(Voucher No:" + record.VoucherNumber + ").,";
                    }
                }
                var Itemcountlist = input.OrderItems.GroupBy(p => p.ItemId).Select(p => new
                {
                    ItemId = p.Key,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    ItemName = p.FirstOrDefault().ItemName,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += item.ItemCode + "-" + item.ItemName + " must be added once in item list.(Current Count:" + item.Count + "),";
                }
                if (input.Id > 0)
                {
                    var dbOrder = orderbookingRepo.GetById(input.Id);
                    var dbdc = dcRepo.GetByOrderId(input.Id);
                    if (dbOrder.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change order no.please use previous order no.(" + dbOrder.VoucherNumber + "),";
                    }
                    if (dbdc != null && dbdc.AccountId != input.AccountId)
                    {
                        err += "can't change party for order.it has already a delivery challan with other party.,";
                    }
                    if (dbOrder.OrderType != input.OrderType)
                    {
                        if (jobORRepo.GetByOrderNo(input.VoucherNumber) != null)
                        {
                            err += "can't change order type.order is used in requisition.";
                        }
                    }
                    if (dbOrder.OrderType == (byte)OrderType.Production && dbOrder.Status != (byte)TransactionStatus.PendingProduction)
                    {

                        err += "Order is already processed and can't be updated.";
                    }
                    if (input.OrderType == (byte)OrderType.Production && dbOrder.OrderType == (byte)OrderType.FinishedGoods && dbOrder.Status != (byte)TransactionStatus.Pending)
                    {
                        err += "Order has already Goods Issue and its type cannot be changed.";
                    }
                    if (input.TransactionType == VoucherType.PurchaseOrder)
                    {
                        record = orderbookingRepo.GetById(input.Id);
                        if (record.SRN != 0 && record.SRN != input.SRN)
                        {
                            err += "can't change SR no.please use previous SR no.(" + record.SRN + "),";
                        }

                    }

                    var GoodsNotes = dcRepo.GetByOrderNumber(goodsNoteType, input.VoucherNumber, input.AuthLocationId)
                  .SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new
                  {
                      ItemId = p.Key,
                      Quantity = p.Sum(q => q.Quantity)
                  }).ToList();
                    var itemIds = input.OrderItems.Select(p => p.ItemId).ToList();
                    var deletedrecord = dbOrder.OrderItems.Where(p => !itemIds.Contains(p.ItemId)).ToList();
                    foreach (var item in dbOrder.OrderItems)
                    {
                        var Dbqty = item.Quantity;//6
                        var Newqty = 0.0M;//2//2-6=-4
                        var Goodsnoteqty = 0.0M;//3
                        if (GoodsNotes.Any(p => p.ItemId == item.ItemId))
                            Goodsnoteqty = GoodsNotes.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;

                        if (input.OrderItems.Any(p => p.ItemId == item.ItemId))
                        {
                            Newqty = input.OrderItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                            if (Newqty < Goodsnoteqty)
                            {
                                err += item.ItemCode + "-" + item.ItemName + " has already Goods note of " + Goodsnoteqty + " quantity.,";
                            }
                        }
                        else if (Goodsnoteqty > 0)
                        {
                            err += item.ItemCode + "-" + item.ItemName + " can't be deleted.it has already Goods note of " + Goodsnoteqty + " quantity.,";

                        }
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
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete the voucher.,";
                    }
                }

                else
                {
                    var orderbookingRepo = new OrderBookingRepository();
                    var jobORRepo = new JobOrderRequisitionRepository(orderbookingRepo);

                    var order = orderbookingRepo.GetById(id);
                    if (order.TransactionType == VoucherType.PurchaseOrder || (order.TransactionType == VoucherType.SaleOrder && order.OrderType == (byte)OrderType.FinishedGoods))
                    {
                        if (order.Status != (byte)TransactionStatus.Pending)
                            err = "Order is already processed.,";
                    }
                    else
                    {
                        if (order.OrderType == (byte)OrderType.Production && order.Status != (byte)TransactionStatus.PendingProduction)
                        {
                            err = "Order is already processed.,";
                        }
                        var requestion = jobORRepo.GetByOrderNo(order.VoucherNumber);
                        if (requestion != null)
                            err += "Order has requisition and can't be deleted.,";
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
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "AccountName", "SalesmanName", "Date", "DeliveryDate" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = (VoucherType)Convert.ToByte(queryString["type"]);
            var key = (VoucherType)Convert.ToByte(queryString["key"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new OrderBookingRepository().AsQueryable(true);
            if (key == VoucherType.Requisition)
            {
                records = records.Where(p => p.TransactionType == type && p.Status == (byte)TransactionStatus.PendingProduction && p.OrderType == (byte)OrderType.Production);
            }
            else if (key == VoucherType.Services)
            {
                switch (type)
                {
                    case VoucherType.CustomerServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.CustomerServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    case VoucherType.SiteServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.SiteServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    case VoucherType.RepairingServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.RepairingServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    default:
                        records = records.Where(p => p.TransactionType == VoucherType.CustomerServiceOrder || p.TransactionType == VoucherType.SiteServiceOrder || p.TransactionType == VoucherType.RepairingServiceOrder && (p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending));
                        break;
                }

            }
            else
            {
                records = records.Where(p => p.OrderType == (byte)OrderType.FinishedGoods);
            }
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.AccountName.Contains(search) ||
                    p.SalesmanName.Contains(search)

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
                data.Add("<input type='text' class='OrderNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                data.Add("<input type='text' class='TransactionType form-control hide' value='" + item.TransactionType + "' />" + item.TransactionType + "");
                data.Add(item.AccountName);
                if (!(key == VoucherType.Services && (type == VoucherType.SiteServiceOrder || type == VoucherType.RepairingServiceOrder)))
                {
                    data.Add(item.SalesmanName);
                }
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add((item.DeliveryDate.HasValue ? item.DeliveryDate.Value.ToString(AppSetting.GridDateFormat) : ""));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        protected JQueryResponse GetDataTableForServices()
        {
            var coloumns = new Object();
            var queryString = Request.RequestUri.ParseQueryString();
            var type = (VoucherType)Convert.ToByte(queryString["type"]);
            var key = (VoucherType)Convert.ToByte(queryString["key"]);
            if (type == VoucherType.CustomerServiceOrder)
                coloumns = new[] { "VoucherNumber", "AccountName", "SalesmanName", "Date" };
            else if (type == VoucherType.SiteServiceOrder)
                coloumns = new[] { "VoucherNumber", "AccountName", "Date" };
            else if (type == VoucherType.RepairingServiceOrder)
                coloumns = new[] { "VoucherNumber", "Date" };

            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);

            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new OrderBookingRepository().AsQueryable(true);
            if (key == VoucherType.Requisition)
            {
                records = records.Where(p => p.TransactionType == type && p.Status == (byte)TransactionStatus.PendingProduction && p.OrderType == (byte)OrderType.Production);
            }
            else if (key == VoucherType.Services)
            {
                switch (type)
                {
                    case VoucherType.CustomerServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.CustomerServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    case VoucherType.SiteServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.SiteServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    case VoucherType.RepairingServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.RepairingServiceOrder && p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending);
                        break;
                    default:
                        records = records.Where(p => p.TransactionType == VoucherType.CustomerServiceOrder || p.TransactionType == VoucherType.SiteServiceOrder || p.TransactionType == VoucherType.RepairingServiceOrder && (p.OrderType == (byte)OrderType.Services && p.Status == (byte)TransactionStatus.Pending));
                        break;
                }

            }
            else
            {
                records = records.Where(p => p.OrderType == (byte)OrderType.FinishedGoods);
            }
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.AccountName.Contains(search) ||
                    p.SalesmanName.Contains(search)

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
                data.Add("<input type='text' class='OrderNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                if (type == VoucherType.CustomerServiceOrder)
                {
                    data.Add("<input type='text' class='TransactionType form-control hide' value='" + item.TransactionType + "' />" + item.AccountName + "");
                    data.Add(item.SalesmanName);
                    data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                }
                else if (type == VoucherType.SiteServiceOrder)
                {
                    data.Add("<input type='text' class='TransactionType form-control hide' value='" + item.TransactionType + "' />" + item.AccountName + "");
                    data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                }
                else if (type == VoucherType.RepairingServiceOrder)
                    data.Add("<input type='text' class='TransactionType form-control hide' value='" + item.TransactionType + "' />" + item.Date.ToString(AppSetting.GridDateFormat) + "");

                rs.aaData.Add(data);
                ////data.Add(item.AccountName);
                //if (!(key p.TransactionType  == VoucherType.Services && (p.TransactionType  == VoucherType.SiteServiceOrder || p.TransactionType  == VoucherType.RepairingServiceOrder)))
                //{

                //}

                //data.Add((item.DeliveryDate.HasValue ? item.DeliveryDate.Value.ToString(AppSetting.GridDateFormat) : ""));

            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
