using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class RequisitionManager
    {
        public static void Save(Requisition requisition)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new JobOrderRequisitionRepository();
                if (requisition.Id == 0)
                {
                    var vouchertypes = new List<VoucherType>();
                    if (requisition.TransactionType  == VoucherType.Requisition)
                    {
                        vouchertypes.Add(VoucherType.Requisition);
                    }
                    else
                    {

                        vouchertypes.Add(VoucherType.CustomerServiceOrder);
                        vouchertypes.Add(VoucherType.SiteServiceOrder);
                        vouchertypes.Add(VoucherType.RepairingServiceOrder);

                    }

                    requisition.VoucherNumber = repo.GetNextVoucherNumber(vouchertypes);
                    repo.Add(requisition, true, false);

                }
                else
                {
                    repo.Update(requisition, repo);

                }
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static Sale GetVocuherDetail(int voucherno, VoucherType transactiontype, string key)
        {
            var d = new Sale();
            bool next, previous;
            d = new SaleRepository().GetByVoucherNumber(voucherno, transactiontype, key, out next, out previous);
            return d;
        }
        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var reqRepo = new JobOrderRequisitionRepository();
                reqRepo.Delete(id);
                reqRepo.SaveChanges();
                scope.Complete();
            }

        }
        public static string ValidateSave(Requisition input)
        {
            var err = ",";
            try
            {
                var reqRepo = new JobOrderRequisitionRepository();
                var orderRepo = new OrderBookingRepository(reqRepo);
                var dcRepo = new DeliveryChallanRepository(reqRepo);
                VoucherType type;
                byte ordertype;
                VoucherType ginptype;
                if (input.TransactionType  == VoucherType.Requisition)
                {
                    type = VoucherType.SaleOrder;
                    ginptype = VoucherType.GINP;
                    ordertype = (byte)OrderType.Production;
                }
                else
                {
                    type = input.TransactionType;
                    ginptype = input.TransactionType;
                    ordertype = (byte)OrderType.Services;

                }
                var record = reqRepo.GetByVoucherNumber(input.VoucherNumber, input.TransactionType, input.Id);
                foreach (var item in input.RequisitionItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var order = orderRepo.GetByVoucherNumber(input.OrderNo, type, ordertype);
                if (order == null)
                {
                    err += "Invalid order number.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date.Value))
                {
                    err += "Date should be within current fiscal year.,";
                }
                //else if(order.OrderType!=(byte)OrderType.Production)
                //{
                //    err += "Invalid order type.only production order can be used in requisition.,";
                //}
                var Itemcountlist = input.RequisitionItems.GroupBy(p => p.ItemId).Select(p => new
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
                    var dbRequestion = reqRepo.GetById(input.Id);
                    if (dbRequestion.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change requestion no.please use previous requestion no.(" + dbRequestion.VoucherNumber + "),";
                    }
                    if (dbRequestion.OrderNo != input.OrderNo)
                    {
                        err += "can't change order no.please use previous order no.(" + dbRequestion.OrderNo + "),";
                    }
                    var GINPList = dcRepo.GetByRequestionNo(ginptype, input.VoucherNumber)
                  .SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new
                  {
                      ItemId = p.Key,
                      Quantity = p.Sum(q => q.Quantity)
                  }).ToList();
                    var itemIds = input.RequisitionItems.Select(p => p.ItemId).ToList();
                    var deletedrecord = dbRequestion.RequisitionItems.Where(p => !itemIds.Contains(p.ItemId)).ToList();
                    foreach (var item in dbRequestion.RequisitionItems)
                    {
                        var Dbqty = item.Quantity;//6
                        var Newqty = 0.0M;//2//2-6=-4
                        var GINPqty = 0.0M;//3
                        if (GINPList.Any(p => p.ItemId == item.ItemId))
                            GINPqty = GINPList.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                        if (input.RequisitionItems.Any(p => p.ItemId == item.ItemId))
                        {
                            Newqty = input.RequisitionItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                            if (Newqty < GINPqty)
                            {
                                err += item.ItemCode + "-" + item.ItemName + " has already Good issue note of " + GINPqty + " quantity.,";
                            }
                        }
                        else if (GINPqty > 0)
                        {
                            err += item.ItemCode + "-" + item.ItemName + " can't be deleted.it has already Good issue note of " + GINPqty + " quantity.,";
                        }
                    }

                    var orderdata = orderRepo.GetByVoucherNumber(input.OrderNo, type, ordertype);
                    if (ordertype == (byte)OrderType.Production && orderdata.Status != (byte)TransactionStatus.PendingProduction)
                    {
                        err += "Order is already processed and requisition can't be updated.";
                    }
                    else if (ordertype == (byte)OrderType.Services && orderdata.Status != (byte)TransactionStatus.Pending)
                    {
                        err += "Order is already processed and requisition can't be updated.";

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
        public static string ValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var reqRepo = new JobOrderRequisitionRepository();
                var orderRepo = new OrderBookingRepository(reqRepo);
                var dcRepo = new DeliveryChallanRepository(reqRepo);

                var requestion = reqRepo.GetById(id);
                /////code for serviceorder addition
                VoucherType type;
                byte ordertype;
                VoucherType ginptype;
                if (requestion.TransactionType  == VoucherType.Requisition)
                {
                    type = VoucherType.SaleOrder;
                    ginptype = VoucherType.GINP;
                    ordertype = (byte)OrderType.Production;
                }
                else
                {
                    type = requestion.TransactionType;
                    ginptype = requestion.TransactionType;
                    ordertype = (byte)OrderType.Services;

                }

                var GINP = dcRepo.GetByRequestionNo(requestion.VoucherNumber, ginptype);
                if (GINP != null)
                {
                    err += "Requisition has goods issue notes and can't be deleted.";
                    var order = orderRepo.GetByVoucherNumber(GINP.OrderNo, type, ordertype);
                    if (order != null)
                    {

                        if (ordertype == (byte)OrderType.Production && order.Status != (byte)TransactionStatus.PendingProduction)
                        {
                            err += "Order is already processed and requisition can't be deleted.";
                        }
                        else if (ordertype == (byte)OrderType.Services && order.Status != (byte)TransactionStatus.Pending)
                        {
                            err += "Order is already processed and requisition can't be deleted.";
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
