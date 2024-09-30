using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.CodeFirst.Models;
using System.Linq;
using System.Text;


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class ServiceOrderController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var vouchertype = new List<VoucherType>() { VoucherType.CustomerServiceOrder,
                VoucherType.SiteServiceOrder, VoucherType.RepairingServiceOrder};
                var currency = new object();
                var queryString = Request.RequestUri.ParseQueryString();
                var type = Convert.ToByte(queryString["type"]);
                var key = queryString["key"].ToLower();
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var orderBookingRepo = new OrderBookingRepository();
                var currencyRepo = new CurrencyRepository(orderBookingRepo);
                var dcRepo = new DeliveryChallanRepository(orderBookingRepo);

                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = orderBookingRepo.GetNextVoucherNumber(vouchertype, locationId);
                var data = orderBookingRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, locationId, out next, out previous);
                var dcs = new object();
                var Avgbalances = new object();
                if (data != null)
                {
                    var balances = new TransactionRepository().GetStockAvgRates(data.OrderItems.Select(p => p.ItemId).Distinct().ToList(),data.Date);
                    foreach (var item in data.OrderItems)
                    {
                        var t = balances.FirstOrDefault(p => p.ItemId == item.ItemId);
                        if (t != null && item.PurchaseRate <= 0)
                        {
                            item.PurchaseRate = t.Rate;
                            item.Amount = item.Quantity * item.Rate;

                        }
                    }
                    currency = currencyRepo.GetCurrencyById(data.CurrencyId);
                    var listDcs = dcRepo.AsQueryable().Where(p => p.OrderId == data.Id).Select(p => new
                    {
                        DcId = p.Id,
                        DcNo = p.VoucherNumber,
                        p.Date,
                        DCItems = p.DCItems.Select(q => new
                        {
                            q.ItemCode,
                            q.ItemId,
                            q.ItemName,
                            q.Quantity,
                            PurchaseRate = 0.0
                        }),
                    }).ToList();

                    var itemIds = listDcs.SelectMany(q => q.DCItems.Select(r => r.ItemId)).ToList();
                    Avgbalances = new TransactionRepository().GetStockAvgRates(itemIds,DateTime.Now);
                    dcs = listDcs;
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        Currency = currency,
                        VoucherNumber = voucherNumber,
                        GINP = dcs,
                        AvgRate = Avgbalances
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        //Next Tax No
        public ApiResponse Get(string keyvalue)
        {
            ApiResponse response;
            try
            {
                var saletaxno = new SaleItemRepository().GetNextSaleTaxNo();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        SaleTaxNo = saletaxno,
                    }
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
                input.OrderType = (byte)OrderType.Services;
                if (input.TransactionType  == VoucherType.RepairingServiceOrder && input.AccountId == 0)
                    input.AccountId = SettingManager.EquipmentAccountId;
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    ServiceOrderManager.Save(input);


                    //if (input.SRN != 0)
                    //    new StockRequisitionRepository().Update(input.SRN);
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
        public ApiResponse Post([FromBody]string key, int orderid)
        {
            ApiResponse response;
            try
            {
                if (orderid > 0)
                {
                    response = new ApiResponse { Success = true, Data = ServiceOrderManager.MarkFinal(orderid) };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = "No record found"
                    };
                }


            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }
        //private ApiResponse GetDataTable()
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var result = "";
        //        var queryString = Request.RequestUri.ParseQueryString();
        //        var vouchertype = Convert.ToByte((queryString["type"]));
        //        var voucher = Numerics.GetInt((queryString["voucher"]));
        //        var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
        //        result = JsonConvert.SerializeObject(data, GetJsonSetting());
        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = result

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
        //    }
        //    return response;
        //}
        public JQueryResponse GetDataTable()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "InvoiceNumber", "Date", "AccountName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var vouchertype = new List<VoucherType>() { VoucherType.CustomerServiceOrder,
                VoucherType.SiteServiceOrder, VoucherType.RepairingServiceOrder};
            var type = queryString["type"];
            var transactiontype =(VoucherType) Convert.ToByte(type);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var customerid = Numerics.GetInt(queryString["FilterCustomer"] + "");
            var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
            var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");
            var repo = new OrderBookingRepository();
            var records = repo.AsQueryable(true);
            if (!string.IsNullOrEmpty(queryString["FromDate"] + "") && !string.IsNullOrEmpty(queryString["ToDate"] + ""))
                records = records.Where(p => p.Date >= fromdate && p.Date <= todate);
            if (!string.IsNullOrEmpty(queryString["type"] + "") && transactiontype > 0)
                records = records.Where(p => p.TransactionType == transactiontype);
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.AccountName.Contains(search) ||
                    (intSearch > 0 && p.VoucherNumber == intSearch)
                     ||
                    (intSearch > 0 && p.InvoiceNumber == intSearch)
                    );
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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.VoucherNumber + "");
                data.Add(item.InvoiceNumber + "");
                data.Add(item.Date.ToString("dd/MM/yyyy"));
                data.Add(item.AccountName);
                var editIcon = "<i class='fa fa-edit' onclick=\"ServiceOrder.Edit(" + item.Id + ",this)\" title='Edit' ></i>";
                //var deleteIcon = "<i class='fa fa-trash-o' onclick=\"VoucherTrans.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                //icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
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
                var type = (queryString["type"]);
                var vouchertype =(VoucherType)Convert.ToByte(type);
                var voucherNo = Numerics.GetInt(queryString["voucherNo"]);
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    var repo = new OrderBookingRepository();
                    repo.Delete(id);
                    repo.SaveChanges();
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
                VoucherType goodsNoteType;
                var orderBookingRepo = new OrderBookingRepository();
                var jobOrderReqRepo = new JobOrderRequisitionRepository(orderBookingRepo);

                if (input.TransactionType  == VoucherType.CustomerServiceOrder || input.TransactionType  == VoucherType.SiteServiceOrder || input.TransactionType  == VoucherType.RepairingServiceOrder)
                {
                    goodsNoteType = VoucherType.GoodIssue;
                }
                else
                {
                    goodsNoteType = VoucherType.GoodReceive;
                }
                var isExists = orderBookingRepo.IsVoucherExists(input.VoucherNumber, input.TransactionType, input.Id);

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
                if (isExists)
                {
                    err += "Voucher no already exist.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Date should be within current fiscal year.,";
                }
                isExists = orderBookingRepo.IsBookNoExits(input.InvoiceNumber, input.TransactionType, input.Id);
                if (isExists)
                {
                    err += "Book no already exist.,";
                }
                var requistion = jobOrderReqRepo.GetByOrderNo(input.InvoiceNumber);

                //record = new OrderBookingRepository().GetBySRN(input.VoucherNumber, VoucherType.SaleOrder, input.Id);
                //if (input.TransactionType  == VoucherType.PurchaseOrder)
                //{
                //    if (record != null)
                //    {
                //        err += "SRN no is already used.(Voucher No:" + record.VoucherNumber + ").,";
                //    }
                //}
                //var Itemcountlist = input.OrderItems.GroupBy(p => p.ItemId).Select(p => new
                //{
                //    ItemId = p.Key,
                //    ItemCode = p.FirstOrDefault().ItemCode,
                //    ItemName = p.FirstOrDefault().ItemName,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();

                //foreach (var item in Itemcountlist)
                //{
                //    err += item.ItemCode + "-" + item.ItemName + " must be added once in item list.(Current Count:" + item.Count + "),";
                //}

                //var servicescountlist = input.OrderServicesItems.GroupBy(p => p.ServiceItemItemId).Select(p => new
                //{
                //    ServiceItemId = p.Key,
                //    ServiceItemCode = p.FirstOrDefault().ServiceItemCode,
                //    ServiceItemName = p.FirstOrDefault().ServiceItemName,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();
                //foreach (var item in servicescountlist)
                //{
                //    err += item.ServiceItemCode + "-" + item.ServiceItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                //}
                if (input.Id > 0)
                {
                    var dbOrder = orderBookingRepo.GetById(input.Id);
                    if (dbOrder.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change order no.please use previous order no.(" + dbOrder.VoucherNumber + "),";
                    }
                    if (dbOrder.OrderType != input.OrderType)
                    {
                        if (jobOrderReqRepo.GetByOrderNo(input.VoucherNumber) != null)
                        {
                            err += "can't change order type.order is used in requisition.";
                        }
                    }
                    //if (dbOrder.OrderType == (byte)OrderType.Services && dbOrder.Status != (byte)TransactionStatus.Pending)
                    //{

                    //    err += "Order is already processed and can't be updated.";
                    //}



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
                var orderBookingRepo = new OrderBookingRepository();
                var jobOrderReqRepo = new JobOrderRequisitionRepository(orderBookingRepo);

                var order = orderBookingRepo.GetById(id);
                if (order.TransactionType  == VoucherType.CustomerServiceOrder || order.TransactionType  == VoucherType.SiteServiceOrder || order.TransactionType  == VoucherType.RepairingServiceOrder)
                {
                    if (order.Status != (byte)TransactionStatus.Pending)
                        err = "Order has already processed.,";
                    var voucherTypes = new List<VoucherType>() { VoucherType.CustomerServiceOrder, VoucherType.SiteServiceOrder, VoucherType.RepairingServiceOrder };
                    var requestion = jobOrderReqRepo.GetByOrderNo(order.VoucherNumber, voucherTypes);
                    if (requestion != null)
                        err += "Order has requisition and can't be deleted.,";
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
