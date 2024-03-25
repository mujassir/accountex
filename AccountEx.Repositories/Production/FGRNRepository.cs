using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class FGRNRepository : GenericRepository<DeliveryChallan>
    {

        public DeliveryChallan GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == vtype);
        }
        public void Save(DeliveryChallan fgrn, FGRNRepository repo)
        {
            var wipRepo = new WorkInProgressRepository(repo);
            var orderBookingRepo = new OrderBookingRepository(repo);
            base.Add(fgrn);
            var wip = wipRepo.GetByVoucherNumber(fgrn.WPNo);
            if (wip != null)
                orderBookingRepo.Update(wip.OrderNo, VoucherType.SaleOrder, (byte)AccountEx.Common.TransactionStatus.FGRN);
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public void Update(DeliveryChallan dc, BaseRepository repo)
        {
            var dcItemRepo = new DCItemRepository();
            var dbDc = GetById(dc.Id, true);

            var Ids = dc.DCItems.Select(p => p.Id).ToList();
            var deletedIds = dbDc.DCItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            dcItemRepo.Delete(deletedIds);
            dcItemRepo.Save(dc.DCItems.ToList());
            base.Update(dc, true, false);

            //var query = "Delete from DCItems where DeliveryChallanId=" + dc.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            //foreach (var item in dc.DCItems)
            //{
            //    Db.DCItems.Add(item);
            //}
            //dc.DCItems = null;
            //base.Update(dc);
            //SaveLog(dc, ActionType.Updated);
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

                var fgrn = FiscalCollection.FirstOrDefault(p => p.Id == id);
                var wip = new WorkInProgressRepository().GetByVoucherNumber(fgrn.WPNo);
                if (wip != null)
                    new OrderBookingRepository().Update(wip.OrderNo, VoucherType.SaleOrder, (byte)AccountEx.Common.TransactionStatus.Ready);
                Db.DeliveryChallans.Remove(fgrn);
                SaveLog(fgrn, ActionType.Deleted);
                Db.SaveChanges();
                scope.Complete();
            }
        }


    }
}