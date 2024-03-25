using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AccountEx.Repositories
{
    public class JobOrderRequisitionRepository : GenericRepository<Requisition>
    {
        public JobOrderRequisitionRepository() :base() { }
        public JobOrderRequisitionRepository(BaseRepository repo) 
        {
            base.Db = repo.GetContext();
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            //return Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        //over load for serviceorder
        public int GetNextVoucherNumber(List<VoucherType> vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => vouchertype.Contains(p.TransactionType)))
                return maxnumber;
            return FiscalCollection.Where(p => vouchertype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public Requisition GetByVoucherNumber(int voucherno, VoucherType type)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type);
        }
        public bool CheckIsVoucherNumberExist(int voucherno, VoucherType type)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == type);
        }
        public Requisition GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public Requisition GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            Requisition v = null;
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
            if (v == null && !Collection.Any(p => p.TransactionType == vtype))
            {
                v = new Requisition();
                v.VoucherNumber = 1001;
                //v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
                //v.CreatedDate = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }


        //overload for service order
        public Requisition GetByVoucherNumber(int voucherno, List<VoucherType> vtype, string key, out bool next, out bool previous)
        {
            Requisition v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => vtype.Contains(p.TransactionType)))
            {
                v = new Requisition();
                v.VoucherNumber = 1001;
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno);
            return v;
        }


        //public override void Save(Requisition requisition)
        //{
        //    if (requisition.OrderNo > 0)
        //    {
        //        var order = AsQueryable<Order>(true).FirstOrDefault(p => p.VoucherNumber == requisition.OrderNo && p.TransactionType == (requisition.TransactionType  == VoucherType.GoodIssue ? VoucherType.SaleOrder : VoucherType.PurchaseOrder));
        //        foreach (var item in order.OrderItems)
        //        {
        //            var dcitem = requisition.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId);
        //            if (dcitem != null)
        //            {
        //                item.QtyDelivered += dcitem.Quantity;
        //                if (item.QtyDelivered >= item.Quantity)
        //                    item.Status = (byte)TransactionStatus.Delivered;
        //                else if (item.QtyDelivered > 0)
        //                    item.Status = (byte)TransactionStatus.PartialyDelivered;
        //                else
        //                    item.Status = (byte)TransactionStatus.Pending;
        //            }
        //        }
        //        if (order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered))
        //            order.Status = (byte)TransactionStatus.PartialyDelivered;
        //        else if (!order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.PartialyDelivered) && !order.OrderItems.Any(p => p.Status == (byte)TransactionStatus.Pending))
        //            order.Status = (byte)TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delived if all of its items status is delivered
        //        //order.Status = (byte)TransactionStatus.PartialyDelivered;
        //        else
        //            order.Status = (byte)TransactionStatus.Pending;
        //    }
        //    requisition.Status = (byte)TransactionStatus.Pending;
        //    base.Save(requisition);
        //    SaveLog(requisition, ActionType.Added);
        //    SaveChanges();
        //}


        public  void Update(Requisition requisition,BaseRepository repo)
        {
            var reqRepo = new JobOrderRequisitionRepository(repo);
            var reqItemsRepo = new RequisitionItemRepository(repo);
            var dcRepo = new DeliveryChallanRepository(repo);

            var dbO = GetById(requisition.Id,true);
           
                var Ids = requisition.RequisitionItems.Select(p => p.Id).ToList();
                var deletedIds = dbO.RequisitionItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
                reqItemsRepo.Delete(deletedIds);
                /////code for serviceorder addition
                var type = new List<VoucherType>();
                if (requisition.TransactionType  == VoucherType.Requisition)
                    type.Add(VoucherType.GINP);
                else
                    type = new List<VoucherType> { VoucherType.CustomerServiceOrder, VoucherType.SiteServiceOrder, VoucherType.RepairingServiceOrder };

                var GINPList = dcRepo.GetByRequestionNo(type, requisition.VoucherNumber)
                  .SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new
                  {
                      ItemId = p.Key,
                      Quantity = p.Sum(q => q.Quantity)
                  }).ToList();
                foreach (var reqisitionItem in requisition.RequisitionItems)
                {
                    var ginp = GINPList.FirstOrDefault(p => p.ItemId == reqisitionItem.ItemId);
                    if (ginp != null)
                    {
                        reqisitionItem.QtyDelivered = ginp.Quantity;

                    }
                    if (reqisitionItem.QtyDelivered >= reqisitionItem.Quantity)
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                    else if (reqisitionItem.QtyDelivered > 0)
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                    else
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                    reqisitionItem.ModifiedAt = DateTime.Now;
                    reqisitionItem.ModifiedBy = SiteContext.Current.User.Id;
                }
                //row added during shared connection implementation
                reqItemsRepo.Save(requisition.RequisitionItems.ToList());
                if (requisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                    requisition.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                else if (!requisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !requisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                    requisition.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                else
                    requisition.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                requisition.RequisitionItems = null;

                //row added during shared connection implementation
                base.Save(requisition, true, false);
                
            
        }
        public Requisition GetByOrderNo(int orderno)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderno);
        }
        public Requisition GetByOrderNo(int orderno, VoucherType vouherType)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderno && p.TransactionType == vouherType);
        }
        public Requisition GetByOrderNo(int orderno, List<VoucherType> voucherTypes)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderno && voucherTypes.Contains(p.TransactionType));
        }
        public Requisition GetByVoucherNumber(int voucherno)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType  == VoucherType.Requisition);
        }
        public Requisition GetByVoucherNo(int voucherno, int id, VoucherType transtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id);
        }
    }
}
