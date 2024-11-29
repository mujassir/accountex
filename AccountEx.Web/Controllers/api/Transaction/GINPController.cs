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
    public class GINPController : BaseApiController
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
                var repo = new GINPRepository();
                var orderBookingRepo = new OrderBookingRepository(repo);
                var JoborderReqRepo = new JobOrderRequisitionRepository(repo);

                var voucherNumber = id;
                var invoiceNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (VoucherType)Convert.ToByte(queryString["type"]);
                var key = queryString["key"].ToLower();
                //var vouchertype =(VoucherType)Convert.ToByte(type);
                var vouchertype = new List<VoucherType>();
                /////code for serviceorder addition
                if (type == VoucherType.GINP)
                {
                    vouchertype.Add(VoucherType.GINP);
                }
                else
                {
                    vouchertype.Add(VoucherType.CustomerServiceOrder);
                    vouchertype.Add(VoucherType.SiteServiceOrder);
                    vouchertype.Add(VoucherType.RepairingServiceOrder);
                }
                ////end
                Object orderinfo = "";
                Object requisitioninfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = repo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = repo.GetNextInvoiceNumber(vouchertype);
                }
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                if (data != null)
                {
                    orderinfo = orderBookingRepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p => new
                    {
                        p.Date,
                        p.DeliveryDate,
                    }
                    ).FirstOrDefault();

                    requisitioninfo = JoborderReqRepo.AsQueryable(true).Where(p => p.Id == data.RequisitionId).Select(p => new
                    {
                        p.Date,
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
                        RequisitionDate = requisitioninfo,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
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
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    DCManager.SaveGINP(input, true);
                    response = new ApiResponse { Success = true, Data = input.Id };
                    //input.FiscalId = SiteContext.Current.Fiscal.Id;
                    //DCManager.SaveGINP(input, false);
                    //var repo = new GINPRepository();
                    //if (input.Id == 0)
                    //{
                    //    DCManager.SaveGINP(input, false);
                    // //  repo.Save(input);
                    //}
                    //else
                    //    repo.Update(input,repo);
                    //response = new ApiResponse { Success = true, Data = input.Id };
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
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    new GINPRepository().Delete(id);
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
            var coloumns = new[] { "OrderNo", "OrderDate", "VoucherNumber", "Date" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = (VoucherType)Convert.ToByte(queryString["type"]);
            var search = (queryString["sSearch"] + "").Trim();
            search = search.ToLower();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new OrderBookingRepository().GetPendingOrderForWIP(type);
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.AccountCode.ToLower().Contains(search) ||
                    p.AccountName.ToLower().Contains(search)

                    );
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
            var sb = new StringBuilder();
            sb.Clear();
            var rs = new JQueryResponse();
            foreach (var item in filteredList)
            {
                var data = new List<string>();
                data.Add("<input type='text' class='OrderNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add(item.VoucherNumber + "");
                data.Add(item.AccountCode + "-" + item.AccountName);
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
                var orderRepo = new OrderBookingRepository(repo);
                var jobReqRepo = new JobOrderRequisitionRepository(repo);
                /////code for serviceorder addition
                VoucherType voucherType;
                byte ordertype;
                VoucherType requisitiontype;

                var voucherTypes = new List<VoucherType>();
                /////code for serviceorder addition

                if (input.TransactionType == VoucherType.GINP)
                {
                    voucherType = VoucherType.SaleOrder;
                    requisitiontype = VoucherType.Requisition;
                    ordertype = (byte)OrderType.Production;
                    voucherTypes.Add(VoucherType.GINP);
                }
                else
                {
                    voucherType = input.TransactionType;
                    requisitiontype = input.TransactionType;
                    ordertype = (byte)OrderType.Services;
                    voucherTypes.Add(VoucherType.CustomerServiceOrder);
                    voucherTypes.Add(VoucherType.SiteServiceOrder);
                    voucherTypes.Add(VoucherType.RepairingServiceOrder);

                }
                var record = repo.CheckIsVoucherNumberExist(input.VoucherNumber, voucherTypes, input.Id);
                if (record)
                {
                    err += "Voucher no already exist.,";
                }
                foreach (var item in input.DCItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                record = repo.CheckIsBookNumberExist(input.InvoiceNumber, input.TransactionType, input.Id);

                if (record)
                {
                    err += "Book no already exist.,";
                }


                /////code for serviceorder addition
                var requestion = jobReqRepo.GetByVoucherNumber(input.RequisitionNo, requisitiontype);

                if (requestion == null)
                {
                    err += "Invalid requisition no.,";
                }
                //if order is processed, then no one GINP can be added
                if (input.Id == 0)
                {
                    var order = orderRepo.GetByVoucherNumber(input.OrderNo, voucherType, ordertype, 0);
                    if (ordertype == (byte)OrderType.Production && order.Status != (byte)TransactionStatus.PendingProduction)
                    {
                        err += "Order is already processed and good issue note can't be added.";
                    }
                    else if (ordertype == (byte)OrderType.Services && order.Status != (byte)TransactionStatus.Pending)
                    {
                        err += "Order is already processed and good issue note can't be added.";
                    }
                }

                var Itemcountlist = input.DCItems.GroupBy(p => p.ItemId).Select(p => new
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

                /////code for serviceorder addition
                var requisitionGoodsNotes = repo.GetByRequestionNo(input.TransactionType, input.RequisitionNo)
                 .Where(p => p.VoucherNumber != input.VoucherNumber).SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new
                 {
                     ItemId = p.Key,
                     Quantity = p.Sum(q => q.Quantity)
                 }).ToList();
                foreach (var item in input.DCItems)
                {
                    var dbQty = item.Quantity;
                    var currentQty = item.Quantity;
                    var GINPqty = 0.0M;
                    var totalQty = 0.0M;
                    if (requisitionGoodsNotes.Any(p => p.ItemId == item.ItemId))
                        GINPqty = requisitionGoodsNotes.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                    totalQty = GINPqty + currentQty;
                    if (requestion.RequisitionItems.Any(p => p.ItemId == item.ItemId))
                    {
                        var requisitionQty = requestion.RequisitionItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                        if (totalQty > requisitionQty)
                        {
                            err += item.ItemCode + "-" + item.ItemName + " maximum quantity reached.(total:" + requisitionQty + " delivered:" + GINPqty + ").remaining quantity is " + (requisitionQty - GINPqty) + ",";
                        }
                    }
                    else
                    {
                        err += item.ItemCode + "-" + item.ItemName + " is not included in current requisition.,";
                    }
                }
                if (input.Id > 0)
                {
                    var dbGoodsNotes = repo.GetById(input.Id);
                    if (dbGoodsNotes.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no no.please use previous voucher no.(" + dbGoodsNotes.VoucherNumber + "),";
                    }

                    if (dbGoodsNotes.OrderNo != input.OrderNo)
                    {
                        err += "can't change order no.please use previous order no.(" + dbGoodsNotes.OrderNo + "),";
                    }

                    /////code for serviceorder addition
                    var orderdata = new OrderBookingRepository().GetByVoucherNumber(input.OrderNo, voucherType, ordertype, 0);

                    if (ordertype == (byte)OrderType.Production && orderdata.Status != (byte)TransactionStatus.PendingProduction)
                    {
                        err += "Order is already processed and good issue note can't be added.";
                    }
                    else if (ordertype == (byte)OrderType.Services && orderdata.Status != (byte)TransactionStatus.Pending)
                    {
                        err += "Order is already processed and good issue note can't be added.";
                    }
                    if (ordertype == (byte)OrderType.Services)
                    {

                        var orderItem = orderdata.OrderItems.Where(p => p.DcId == input.Id).GroupBy(p => p.ItemId).Select(p => new
                        {
                            ItemId = p.Key,
                            Quantity = p.Sum(q => q.Quantity)
                        }).ToList();
                        var itemIds = input.DCItems.Select(p => p.ItemId).ToList();
                        var deletedrecord = dbGoodsNotes.DCItems.Where(p => !itemIds.Contains(p.ItemId)).ToList();
                        foreach (var item in dbGoodsNotes.DCItems)
                        {
                            var Dbqty = item.Quantity;//6
                            var Newqty = 0.0M;//2//2-6=-4
                            var orderQty = 0.0M;//3
                            if (orderItem.Any(p => p.ItemId == item.ItemId))
                                orderQty = orderItem.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;

                            if (input.DCItems.Any(p => p.ItemId == item.ItemId))
                            {
                                Newqty = input.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                                if (Newqty < orderQty)
                                {
                                    err += item.ItemCode + "-" + item.ItemName + " has already order of " + orderQty + " quantity.,";
                                }
                            }
                            else if (orderQty > 0)
                            {
                                err += item.ItemCode + "-" + item.ItemName + " can't be deleted.it has already order of " + orderQty + " quantity.,";

                            }
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

                var GINP = new DeliveryChallanRepository().GetById(id);
                /////code for serviceorder addition
                if (GINP != null)
                {
                    VoucherType type;
                    byte ordertype;
                    if (GINP.TransactionType == VoucherType.GINP)
                    {
                        type = VoucherType.SaleOrder;
                        ordertype = (byte)OrderType.Production;
                    }
                    else
                    {
                        type = GINP.TransactionType;
                        ordertype = (byte)OrderType.Services;
                    }
                    var order = new OrderBookingRepository().GetByVoucherNumber(GINP.OrderNo, type, ordertype, 0);
                    if (order != null)
                    {
                        if (ordertype == (byte)OrderType.Production && order.Status != (byte)TransactionStatus.PendingProduction)
                        {
                            err += "Order is already processed and good issue note can't be deleted.";
                        }
                        else if (ordertype == (byte)OrderType.Services && order.Status != (byte)TransactionStatus.Pending)
                        {
                            err += "Order is already processed and good issue note can't be deleted.";
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
    }
}
