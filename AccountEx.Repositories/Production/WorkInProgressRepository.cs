using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Production;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AccountEx.Repositories
{
    public class WorkInProgressRepository : GenericRepository<WorkInProgress>
    {
        public WorkInProgressRepository() : base() { }
        public WorkInProgressRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public IList<WPItemWithParentDetail> GetWithParentDetail(Expression<Func<WPItemWithParentDetail, bool>> predicate)
        {
            var wpItemRepo = new WPItemsRepository(this);
            return wpItemRepo.GetWithParentDetail(predicate).ToList();
        }
        public int GetNextVoucherNumber(VoucherType voucherType=VoucherType.Production)
        {
            const int maxNumber = 1;
            if (!FiscalCollection.Any())
                return maxNumber;
            // return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            var voucher = FiscalCollection.Where(p => p.VoucherType == voucherType).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            if (voucher != null)
            {
                var vno = voucher.VoucherNumber;
                vno = vno + 1;
                return vno;
            }
            return maxNumber;
        }
        public WorkInProgress GetByVoucherNumber(int voucherNo)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherNo);
        }
        public WorkInProgress GetByVoucherNumber(int voucherNo, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherNo && p.Id != id);
        }
        public WorkInProgress GetByBookNumber(int bookNo, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public bool IsVoucherExists(int voucherno, int id, VoucherType voucherType = VoucherType.Production)
        {
            return FiscalCollection.Any(p => p.VoucherType == voucherType && p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsBookNoExists(int bookNo, int id, VoucherType voucherType = VoucherType.Production)
        {
            return FiscalCollection.Any(p => p.VoucherType == voucherType && p.InvoiceNumber == bookNo && p.Id != id);
        }
        public WorkInProgress GetByOrderNumber(int orderNo, int Id)
        {
            return FiscalCollection.FirstOrDefault(p => p.OrderNo == orderNo && p.Id != Id);
        }
        public WorkInProgress GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous, VoucherType vtype = VoucherType.Production)
        {
            WorkInProgress v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherType == vtype && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            next = FiscalCollection.Any(p => p.VoucherType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {
            var wp = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            if (wp != null)
            {
                ObjectSet.Remove(wp);
                new OrderBookingRepository().Update(wp.OrderNo, VoucherType.SaleOrder, (byte)TransactionStatus.PendingProduction);
                Db.SaveChanges();
            }
        }
        public override void Update(WorkInProgress p)
        {
            //var query = "Delete from WPItems where WorkInProgressId=" + p.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            //foreach (var item in p.WPItems)
            //{
            //    Db.WPItem.Add(item);
            //}
            //p.WPItems = null;
            var record = GetById(p.Id, true);
            var ids = p.WPItems.Select(q => q.Id).ToList();
            var deletedIds = record.WPItems.Where(q => !ids.Contains(q.Id)).Select(q => q.Id).ToList();
            new WPItemsRepository(this).Delete(deletedIds);
            new WPItemsRepository(this).Save(p.WPItems.ToList());
            base.Update(p, true, false);
        }


      public WorkInProgress GetByVoucherNumberLabours(int voucherno, string key, out bool next, out bool previous, VoucherType vtype = VoucherType.Labours)
    {
            WorkInProgress v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherType == vtype && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.VoucherType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            next = FiscalCollection.Any(p => p.VoucherType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
    }
}