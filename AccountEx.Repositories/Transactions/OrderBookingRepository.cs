﻿using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class OrderBookingRepository : GenericRepository<Order>
    {
        public OrderBookingRepository() : base() { }
        public OrderBookingRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public Order GetByVoucherNumber(int voucherno, VoucherType type, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.AuthLocationId == locationId);
        }
        public Order GetByVoucherNumber(int voucherno, VoucherType type, int id, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id && p.AuthLocationId == locationId);
        }
        public bool IsVoucherExists(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }

        public Order GetByBookNumber(int bookNo, VoucherType type, int id, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id && p.AuthLocationId == locationId);
        }
        public int GetVoucherNoByBookNumber(int bookNo, VoucherType type, int locationId)
        {
            if (FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.AuthLocationId == locationId))
                return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.AuthLocationId == locationId).VoucherNumber;
            else return 0;
        }
        public Order GetByVoucherNumber(int voucherno, VoucherType type, byte ordertype, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.OrderType == ordertype && p.AuthLocationId == locationId);
        }
        public Order GetBySRN(int srn, VoucherType type, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.SRN == srn && p.TransactionType == type && p.AuthLocationId == locationId);
        }
        public IQueryable<DataTableExtra> GetPendingOrderForWIP(VoucherType type)
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var orders = FiscalCollection.Where(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PendingProduction
              && p.CompanyId == companyId && p.OrderType == (byte)OrderType.Production);

            var records = AsQueryable<DeliveryChallan>(true).Join(orders, ginp => ginp.OrderNo, o => o.VoucherNumber, (ginp, o) => new { GINP = ginp, Order = o }).
           Where(p => p.GINP.TransactionType == type
              && p.GINP.CompanyId == companyId).
               GroupBy(p => p.Order.VoucherNumber).Select(x => new DataTableExtra()
               {
                   VoucherNumber = x.FirstOrDefault().Order.VoucherNumber,
                   AccountCode = x.FirstOrDefault().Order.AccountCode,
                   AccountName = x.FirstOrDefault().Order.AccountName,
                   Date = x.FirstOrDefault().Order.Date,

               }).AsQueryable();
            return records;
        }
        public IQueryable<DataTableExtra> GetPendingOrderForFGRN()
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var orders = FiscalCollection.Where(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Ready
                && p.OrderType == (byte)OrderType.Production);


            var records = AsQueryable<WorkInProgress>(true).Join(orders, wip => wip.OrderNo, o => o.VoucherNumber, (wip, o) => new { WIP = wip, Order = o }).
            Where(p => p.WIP.CompanyId == companyId).
                GroupBy(p => p.Order.VoucherNumber).Select(x => new DataTableExtra()
                {
                    VoucherNumber = x.FirstOrDefault().WIP.VoucherNumber,
                    OrderNumber = x.FirstOrDefault().Order.VoucherNumber,
                    AccountCode = x.FirstOrDefault().Order.AccountCode,
                    AccountName = x.FirstOrDefault().Order.AccountName,
                    Date = x.FirstOrDefault().WIP.Date,

                }).AsQueryable();
            return records;
        }
        //public bool IsOrderProcessed(int orderNo,VoucherType type)
        //{
        //    return FiscalCollection.FirstOrDefault(p => p.VoucherNumber ==orderNo && p.TransactionType == type);
        // }

        public Order GetBySRN(int srn, VoucherType type, int id, int locationId)
        {
            return FiscalCollection.FirstOrDefault(p => p.SRN == srn && p.TransactionType == type && p.Id != id && p.AuthLocationId == locationId);
        }
        public int GetNextVoucherNumber(VoucherType vouchertype, int locationId)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype && p.AuthLocationId == locationId))
                return maxnumber;
            //return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }

        //over load for serviceorder
        public int GetNextVoucherNumber(List<VoucherType> vouchertype, int locationId)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => vouchertype.Contains(p.TransactionType) && p.AuthLocationId == locationId))
                return maxnumber;
            return FiscalCollection.Where(p => vouchertype.Contains(p.TransactionType) && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }

        public override void Update(Order order)
        {
            var dbOrder = GetById(order.Id, true);
            if (order.OrderType == (byte)OrderType.Production)
            {
                if (dbOrder.Status == (byte)AccountEx.Common.TransactionStatus.Pending)
                    order.Status = (byte)AccountEx.Common.TransactionStatus.PendingProduction;
            }
            else if (order.OrderType == (byte)OrderType.FinishedGoods || order.OrderType == (byte)OrderType.Services)
            {
                if (dbOrder.Status == (byte)AccountEx.Common.TransactionStatus.PendingProduction)
                    order.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
            }

            //Changes due ToString service order

            VoucherType voucherType;
            if (order.TransactionType == VoucherType.SaleOrder)
                voucherType = VoucherType.GoodIssue;
            else if (order.TransactionType == VoucherType.PurchaseOrder)
                voucherType = VoucherType.GoodReceive;
            else
                voucherType = order.TransactionType;
            //end//
            if (order.TransactionType == VoucherType.SaleOrder || order.TransactionType == VoucherType.PurchaseOrder)
            {
                var dcList = new DeliveryChallanRepository(this).GetItemWithQuantity(voucherType, order.VoucherNumber);
                foreach (var orderItem in order.OrderItems)
                {
                    var dc = dcList.FirstOrDefault(p => p.ItemId == orderItem.ItemId);
                    if (dc != null)
                    {
                        orderItem.QtyDelivered = dc.Quantity;

                    }
                    if (orderItem.QtyDelivered >= orderItem.Quantity)
                        orderItem.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                    else if (orderItem.QtyDelivered > 0)
                        orderItem.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                    else
                        orderItem.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                }

                if (order.OrderItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                    order.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                else if (!order.OrderItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !order.OrderItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                    order.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                else
                    order.Status =
                        (order.OrderType == (byte)OrderType.Production ? (byte)AccountEx.Common.TransactionStatus.PendingProduction
                        : (byte)AccountEx.Common.TransactionStatus.Pending);
            }
            //add,update & remove services items
            var Ids = order.OrderExpenseItems.Select(p => p.Id).ToList();
            var deletedIds = dbOrder.OrderExpenseItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            new OrderServiceItemRepository(this).Delete(deletedIds);
            new OrderServiceItemRepository(this).Save(false, order.OrderExpenseItems.ToList());

            //add,update & remove order items
            Ids = order.OrderItems.Select(p => p.Id).ToList();
            deletedIds = dbOrder.OrderItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            new OrderItemRepository(this).Delete(deletedIds);
            new OrderItemRepository(this).Save(false, order.OrderItems.ToList());
            base.Update(order, true, false);
        }


        public void Update(int orderno, VoucherType type, byte status, int locationId)
        {

            var order = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == orderno && p.TransactionType == type && p.AuthLocationId == locationId);
            if (order != null)
            {
                order.Status = status;
                Db.SaveChanges();
            }
        }
        public void Update(int orderId, byte status)
        {

            var order = FiscalCollection.FirstOrDefault(p => p.Id == orderId);
            if (order != null)
            {
                order.Status = status;
                Db.SaveChanges();
            }
        }
        public Order GetByVoucherNumber(int voucherno, VoucherType vtype, string key, int locationId, out bool next, out bool previous)
        {
            Order v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno && p.AuthLocationId == locationId);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno && p.AuthLocationId == locationId);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype && p.AuthLocationId == locationId))
            {
                v = new Order();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno && p.AuthLocationId == locationId);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno && p.AuthLocationId == locationId);
            return v;
        }


        //overload for service order
        public Order GetByVoucherNumber(int voucherno, List<VoucherType> vtype, string key, int locationId, out bool next, out bool previous)
        {
            Order v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno && p.AuthLocationId == locationId);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno && p.AuthLocationId == locationId);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.AuthLocationId == locationId))
            {
                v = new Order();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno && p.AuthLocationId == locationId);
            previous = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno && p.AuthLocationId == locationId);
            return v;
        }

        public void DeleteByVoucherNumber(int voucherno, VoucherType type, int locationId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.AuthLocationId == locationId);
                if (record != null)
                {
                    Db.Orders.Remove(record);
                    var SRN = AsQueryable<StockRequisition>(true).FirstOrDefault(p => p.VoucherNumber == record.VoucherNumber && p.AuthLocationId == locationId);
                    if (SRN != null)
                        SRN.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                }
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public override void Delete(int id)
        {
            var record = FiscalCollection.FirstOrDefault(p => p.Id == id);
            if (record != null)
            {
                Db.Orders.Remove(record);
                var SRN = AsQueryable<StockRequisition>(true).FirstOrDefault(p => p.VoucherNumber == record.SRN);
                if (SRN != null)
                    SRN.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
            }
        }

        public List<OrderExtra> GetOrderByStatusesSP(DateTime fromdate, DateTime todate, int vouchertype)
        {
            Db.Database.CommandTimeout = 240;
            var query = string.Format("EXEC [DBO].[GetOrdersByStatuses] @FROMDATE = {0}, @TODATE = {1}, @COMPANYID={2},@FiscalId={3},@VoucherType={4}", "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, vouchertype);
            return Db.Database.SqlQuery<OrderExtra>(query).ToList();
        }

    }
}