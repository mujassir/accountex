using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class GINPRepository : GenericRepository<DeliveryChallan>
    {

        public DeliveryChallan GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == vtype);
        }

        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
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
        public int GetNextInvoiceNumber(List<VoucherType> vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => vouchertype.Contains(p.TransactionType)))
                return maxnumber;
            return FiscalCollection.Where(p => vouchertype.Contains(p.TransactionType)).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }

        public int GetNextInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }

        public override void Save(DeliveryChallan ginp)
        {
                VoucherType vtype;
                if (ginp.TransactionType == VoucherType.GINP)
                {
                    vtype = VoucherType.Requisition;
                }
                else
                {
                    vtype = ginp.TransactionType;
                }
                var dbRequisition = AsQueryable<Requisition>(true).FirstOrDefault(p => p.VoucherNumber == ginp.RequisitionNo && p.TransactionType == vtype);
                foreach (var item in dbRequisition.RequisitionItems)
                {
                    var ginpItem = ginp.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (ginpItem != null)
                    {
                        item.QtyDelivered += ginpItem.Quantity;
                        if (item.QtyDelivered >= item.Quantity)
                            item.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                        else if (item.QtyDelivered > 0)
                            item.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                        else
                            item.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                    }
                }
                if (dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                else if (!dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                else
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                ginp.Status = (byte)AccountEx.Common.TransactionStatus.Pending;

                base.Save(ginp, true, false);
              // base.Save(ginp);
                //SaveLog(ginp, ActionType.Added);
                //SaveChanges();
                //scope.Complete();
            
        }

        public void Update(DeliveryChallan ginp,BaseRepository baseRepo)
        {
            var dbDc = GetById(ginp.Id, true);
            /////code for serviceorder addition
                var type = new List<VoucherType>();
                if(ginp.TransactionType  == VoucherType.GINP)
                    type.Add(VoucherType.Requisition);
                else
                    type = new List<VoucherType> { VoucherType.CustomerServiceOrder, VoucherType.SiteServiceOrder, VoucherType.RepairingServiceOrder };
                ////end
                //var dbGINP = FiscalCollection.FirstOrDefault(p => p.Id == ginp.Id);

                /////code for serviceorder addition

                var dbRequisition = AsQueryable<Requisition>(true).FirstOrDefault(p => p.VoucherNumber == ginp.RequisitionNo && type.Contains(p.TransactionType));
                foreach (var item in dbDc.DCItems)
                {
                    var reqisitionItem = dbRequisition.RequisitionItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (reqisitionItem == null)
                        continue;
                    var dcitem = ginp.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (dcitem == null)
                        reqisitionItem.QtyDelivered -= item.Quantity;
                    else
                        reqisitionItem.QtyDelivered += dcitem.Quantity - item.Quantity;

                    if (reqisitionItem.QtyDelivered >= reqisitionItem.Quantity)
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                    else if (reqisitionItem.QtyDelivered > 0)
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                    else
                        reqisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                }
                if (dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                else if (!dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                else
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                ginp.Status = dbRequisition.Status;
                //var query = "Delete from DCItems where DeliveryChallanId=" + ginp.Id + " AND CompanyId=" + SiteContext.Current.UserCompany ;
                //Db.Database.ExecuteSqlCommand(query);
                //foreach (var item in ginp.DCItems)
                //{
                //    Db.DCItems.Add(item);
                //}

                var dcItemRepo = new DCItemRepository(baseRepo);
                var Ids = ginp.DCItems.Select(p => p.Id).ToList();
                var deletedIds = dbDc.DCItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
                dcItemRepo.Delete(deletedIds);
                dcItemRepo.Save(ginp.DCItems.ToList());

                base.Update(ginp, true, false);

                //SaveChanges();
                //ginp.DCItems = null;

                //Db.Entry(dbGINP).CurrentValues.SetValues(ginp);
                //SaveLog(ginp, ActionType.Updated);
                //SaveChanges();
                //scope.Complete();
            

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
            if (v == null && !Collection.Any(p => p.TransactionType == vtype))
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

        //overload for service order
        public DeliveryChallan GetByVoucherNumber(int voucherno, List<VoucherType> vtype, string key, out bool next, out bool previous)
        {
            DeliveryChallan v = null;
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
                v = new DeliveryChallan();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
                v.CreatedDate = DateTime.Now;
            }
            next = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno);
            return v;
        }

        
        
        public void DeleteByVoucherNumber(int voucherno)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                //string query = "Delete from DeliveryChallans where VoucherNumber='" + voucherno + "'";
                var dcs = FiscalCollection.Where(p => p.VoucherNumber == voucherno);

                foreach (var item in dcs)
                {
                    Db.DeliveryChallans.Remove(item);
                }
                //Db.Database.ExecuteSqlCommand(query);
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public override void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {


                var dbGINP = FiscalCollection.FirstOrDefault(p => p.Id == id);
                var dbRequisition = AsQueryable<Requisition>(true).FirstOrDefault(p => p.VoucherNumber == dbGINP.RequisitionNo && p.TransactionType == dbGINP.TransactionType);
                foreach (var item in dbGINP.DCItems)
                {
                    var requisitionItem = dbRequisition.RequisitionItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (requisitionItem == null)
                        continue;
                    requisitionItem.QtyDelivered -= item.Quantity;


                    if (requisitionItem.QtyDelivered >= requisitionItem.Quantity)
                        requisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                    else if (requisitionItem.QtyDelivered > 0)
                        requisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                    else
                        requisitionItem.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                }
                if (dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                else if (!dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dbRequisition.RequisitionItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                else
                    dbRequisition.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                Db.DeliveryChallans.Remove(dbGINP);
                SaveLog(dbGINP,ActionType.Deleted);
                Db.SaveChanges();
                scope.Complete();
            }
        }




    }
}