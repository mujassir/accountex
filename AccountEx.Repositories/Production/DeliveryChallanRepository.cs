using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class DeliveryChallanRepository : GenericRepository<DeliveryChallan>
    {
        public DeliveryChallanRepository() : base() { }
        public DeliveryChallanRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public DeliveryChallan GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == vtype);
        }
        public DeliveryChallan GetByOrderNumber(int orderNo, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderNo && p.TransactionType == vtype);
        }
        public DeliveryChallan GetByOrderId(int orderId)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderId == orderId);
        }
        public List<DeliveryChallan> GetByOrderNumber(VoucherType vtype, int orderNo)
        {
            return FiscalCollection.Where(p => p.OrderNo == orderNo && p.TransactionType == vtype).ToList();
        }
        public List<DcWithQuantityExtra> GetItemWithQuantity(VoucherType vtype, int orderNo)
        {
            return FiscalCollection.Where(p => p.OrderNo == orderNo && p.TransactionType == vtype).SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new DcWithQuantityExtra()
            {
                ItemId = p.Key,
                Quantity = p.Sum(q => q.Quantity)
            }).ToList();
        }
        public DeliveryChallan GetByOrderNumber(int orderNo, VoucherType vtype, int Id)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderNo && p.TransactionType == vtype && p.Id != Id);
        }

        public DeliveryChallan GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public bool CheckIsVoucherNumberExist(int voucherno, VoucherType type, int id)
        {
            return CheckIsVoucherNumberExist(voucherno, new List<VoucherType> { type }, id);
        }
        public bool CheckIsVoucherNumberExist(int voucherno, List<VoucherType> types, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && types.Contains(p.TransactionType) && p.Id != id);
        }
        public DeliveryChallan GetByBookNumber(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }
        public bool CheckIsBookNumberExist(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }
        public DeliveryChallan GetByWPNo(int wpNo, VoucherType vtype, int Id)
        {
            return FiscalCollection.FirstOrDefault(p => p.WPNo == wpNo && p.TransactionType == vtype && p.Id != Id);
        }

        public DeliveryChallan GetByRequestionNo(int requestionNo, VoucherType type)
        {
            return FiscalCollection.FirstOrDefault(p => p.RequisitionNo == requestionNo && p.TransactionType == type);
        }
        public List<DeliveryChallan> GetByRequestionNo(VoucherType type, int requestionNo)
        {
            return FiscalCollection.Where(p => p.RequisitionNo == requestionNo && p.TransactionType == type).ToList();
        }
        public List<DeliveryChallan> GetByRequestionNo(List<VoucherType> type, int requestionNo)
        {
            return FiscalCollection.Where(p => p.RequisitionNo == requestionNo && type.Contains(p.TransactionType)).ToList();
        }
        public bool IsOrderLinkWithOtherParty(int ordrId, int accountId)
        {
            return FiscalCollection.Any(p => p.OrderId == ordrId && p.AccountId != accountId);
        }
        public bool IsDcLinkWithOtherParty(List<int> dcIds, int accountId)
        {
            return FiscalCollection.Any(p => dcIds.Contains(p.Id) && p.AccountId != accountId);
        }
        public bool CheckIfDcExist(int id)
        {
            return Collection.Any(p => p.Id == id);
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public override void Save(DeliveryChallan dc)
        {
            if (dc.OrderNo > 0)
            {
                var order = AsQueryable<Order>(true).FirstOrDefault(p => p.VoucherNumber == dc.OrderNo && p.TransactionType == (dc.TransactionType  == VoucherType.GoodIssue ? VoucherType.SaleOrder : VoucherType.PurchaseOrder));
                foreach (var item in order.OrderItems)
                {
                    var dcitem = dc.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (dcitem != null)
                    {
                        item.QtyDelivered += dcitem.Quantity;
                        if (item.QtyDelivered >= item.Quantity)
                            item.Status = (byte)TransactionStatus.Delivered;
                        else if (item.QtyDelivered > 0)
                            item.Status = (byte)TransactionStatus.PartialyDelivered;
                        else
                            item.Status = (byte)TransactionStatus.Pending;
                    }
                }
                if (order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered))
                    order.Status = (byte)TransactionStatus.PartialyDelivered;
                else if (!order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered) && !order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.Pending))
                    order.Status = (byte)TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delived if all of its items status is delivered
                //order.Status = (byte)TransactionStatus.PartialyDelivered;
                else
                    order.Status = (byte)TransactionStatus.Pending;
            }
            dc.Status = (byte)TransactionStatus.Pending;


            base.Save(dc, true, false);

        }
        public void Update(DeliveryChallan dc, BaseRepository baseRepo)
        {
            var salerepo = new SaleRepository(baseRepo);
            var dbDc = GetById(dc.Id, true);

            if (dc.OrderNo > 0)
            {
                var order = AsQueryable<Order>(true).FirstOrDefault(p => p.VoucherNumber == dc.OrderNo && p.TransactionType == (dc.TransactionType  == VoucherType.GoodIssue ? VoucherType.SaleOrder : VoucherType.PurchaseOrder));
                foreach (var dbDcItem in dbDc.DCItems)
                {
                    var orderitem = order.OrderItems.FirstOrDefault(p => p.ItemId == dbDcItem.ItemId);

                    if (orderitem == null)
                        continue;
                    var dcitem = dc.DCItems.FirstOrDefault(p => p.ItemId == dbDcItem.ItemId);
                    if (dcitem == null)
                        orderitem.QtyDelivered -= dbDcItem.Quantity;
                    else
                        orderitem.QtyDelivered += dcitem.Quantity - dbDcItem.Quantity;

                    if (orderitem.QtyDelivered >= orderitem.Quantity)
                        orderitem.Status = (byte)TransactionStatus.Delivered;
                    else if (orderitem.QtyDelivered > 0)
                        orderitem.Status = (byte)TransactionStatus.PartialyDelivered;
                    else
                        orderitem.Status = (byte)TransactionStatus.Pending;


                }
                if (order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered))
                    order.Status = (byte)TransactionStatus.PartialyDelivered;
                else if (!order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered) && !order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.Pending))
                    order.Status = (byte)TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delived if all of its items status is delivered
                //order.Status = (byte)TransactionStatus.PartialyDelivered;
                else
                    order.Status = (byte)TransactionStatus.Pending;
            }
            //var query = "Delete from DCItems where DeliveryChallanId=" + dc.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            var voucherTypes = new List<VoucherType>();
            if (dc.TransactionType  == VoucherType.GoodIssue)
            {
                voucherTypes = new List<VoucherType>() { VoucherType.Sale, VoucherType.GstSale };
            }
            else
            {
                voucherTypes = new List<VoucherType>() { VoucherType.Purchase, VoucherType.GstPurchase };
            }
            var saleList = salerepo.GetByDCNo(voucherTypes, dc.VoucherNumber)
          .SelectMany(p => p.SaleItems).GroupBy(p => p.ItemId).Select(p => new
          {
              ItemId = p.Key,
              Quantity = p.Sum(q => q.Quantity)
          }).ToList();
            foreach (var item in dc.DCItems)
            {

                //Update SaleITem Status after dc has been changed


                var saleItem = saleList.FirstOrDefault(p => p.ItemId == item.ItemId);
                if (saleItem == null)
                    item.QtyDelivered = 0;
                else
                    item.QtyDelivered = saleItem.Quantity;

                if (item.QtyDelivered >= item.Quantity)
                    item.Status = (byte)TransactionStatus.Delivered;
                else if (item.QtyDelivered > 0)
                    item.Status = (byte)TransactionStatus.PartialyDelivered;
                else
                    item.Status = (byte)TransactionStatus.Pending;
                //Db.DCItems.Add(item);
            }
            if (dc.DCItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered))
                dc.Status = (byte)TransactionStatus.PartialyDelivered;
            else if (!dc.DCItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered) && !dc.DCItems.Any(p => p.Status == (byte)TransactionStatus.Pending))
                dc.Status = (byte)TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delived if all of its items status is delivered
            //order.Status = (byte)TransactionStatus.PartialyDelivered;
            else
                dc.Status = (byte)TransactionStatus.Pending;


            var dcItemRepo = new DCItemRepository(baseRepo);


            var Ids = dbDc.DCItems.Select(p => p.Id).ToList();
            var deletedIds = dbDc.DCItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            dcItemRepo.Delete(deletedIds);
            dcItemRepo.Save(dc.DCItems.ToList());

            base.Update(dc, true, false);
        }
        public DeliveryChallan GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            DeliveryChallan v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype))
            {
                v = new DeliveryChallan();
                v.VoucherNumber = 1;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
                v.CreatedDate = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {

            //string query = "Delete from DeliveryChallans where VoucherNumber='" + voucherno + "'";
            var dcs = FiscalCollection.Where(p => p.VoucherNumber == voucherno);
            foreach (var item in dcs)
            {
                Db.DeliveryChallans.Remove(item);
            }
            //Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();

        }
        public override void Delete(int id)
        {


            var dc = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(dc);
        }
        public void Delete(DeliveryChallan dc)
        {


            if (dc.OrderNo > 0)
            {
                var order = AsQueryable<Order>(true).FirstOrDefault(p => p.VoucherNumber == dc.OrderNo && p.TransactionType == (dc.TransactionType  == VoucherType.GoodIssue ? VoucherType.SaleOrder : VoucherType.PurchaseOrder));
                if (order != null)
                {
                    foreach (var item in dc.DCItems)
                    {
                        var orderitem = order.OrderItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                        if (orderitem == null)
                            continue;
                        orderitem.QtyDelivered -= item.Quantity;


                        if (orderitem.QtyDelivered >= orderitem.Quantity)
                            orderitem.Status = (byte)TransactionStatus.Delivered;
                        else if (orderitem.QtyDelivered > 0)
                            orderitem.Status = (byte)TransactionStatus.PartialyDelivered;
                        else
                            orderitem.Status = (byte)TransactionStatus.Pending;
                    }
                    if (order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered))
                        order.Status = (byte)TransactionStatus.PartialyDelivered;
                    else if (!order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered) && !order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.Pending))
                        order.Status = (byte)TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delivered if all of its items status is delivered
                                                                             //order.Status = (byte)TransactionStatus.PartialyDelivered;
                    else
                        order.Status = (byte)TransactionStatus.Pending;
                }
            }

            string query = "Delete from DeliveryChallans where Id=" + dc.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId + "";
            Db.Database.ExecuteSqlCommand(query);
            SaveLog(dc, ActionType.Deleted);
            Db.SaveChanges();
        }
        public DeliveryChallan GetByVoucherNo(int voucherno, int id, VoucherType transtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id);
        }

        public IQueryable<DataTableExtra> GetPendingOrderForWIP(VoucherType type)
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var orders = AsQueryable<Order>(true).Where(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PendingProduction
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

        public IQueryable<DataTableExtra> GetPendingDeliveryChallan(VoucherType type)
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var orders = AsQueryable<Order>(true);

            var records = AsQueryable<DeliveryChallan>(true).Join(orders, dc => dc.OrderId, o => o.Id, (deliveryChallan, o) => new { DC = deliveryChallan, Order = o }).
           Where(p => p.DC.TransactionType == type && p.DC.Status != (byte)TransactionStatus.Delivered).Select(x => new DataTableExtra()
           {
               Id = x.DC.Id,
               VoucherNumber = x.DC.VoucherNumber,
               OrderNumber = x.Order.VoucherNumber,
               AccountCode = x.Order.AccountCode,
               AccountName = x.Order.AccountName,
               Date = x.DC.Date,

           }).AsQueryable();
            return records;
        }
        public List<DataTableExtra> GetPendingDeliveryChallan(VoucherType type, bool loadList)
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var orders = AsQueryable<Order>(true);

            var records = AsQueryable<DeliveryChallan>(true).LeftOuterJoin(orders, dc => dc.OrderId, o => o.Id, (deliveryChallan, o) => new { DC = deliveryChallan, Order = o }).
           Where(p => p.DC.TransactionType == type && p.DC.Status != (byte)TransactionStatus.Delivered).Select(x => new DataTableExtra()
           {
               Id = x.DC.Id,
               VoucherNumber = x.DC.VoucherNumber,
               OrderNumber = x.Order.VoucherNumber,
               AccountCode = x.Order.AccountCode,
               AccountName = x.Order.AccountName,
               Date = x.DC.Date,

           }).ToList();
            return records;
        }

        public IQueryable<DataTableExtra> Get(List<int> dcIds)
        {

            var records = new GenericRepository<vw_PendingDeliveryChallan>().AsQueryable(true).Where(p => dcIds.Contains(p.Id)).Select(p => new DataTableExtra()
            {
                Id = p.Id,
                InvoiceNumber = p.InvoiceNumber != null ? p.InvoiceNumber.Value : 0,
                VoucherNumber = p.VoucherNumber,
                OrderNumber = p.OrderNumber ?? 0,
                AccountCode = p.AccountCode,
                AccountName = p.AccountName,
                Date = p.Date,

            }).AsQueryable();
            return records;


        }


    }
}