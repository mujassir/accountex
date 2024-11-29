using AccountEx.CodeFirst.Models.Stock;
using AccountEx.Common;
using System;
using System.Linq;


namespace AccountEx.Repositories
{
    public class InternalStockTransferRepository : GenericRepository<InternalStockTransfer>
    {
        public InternalStockTransferRepository() : base() { }
        public InternalStockTransferRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public InternalStockTransfer GetByVoucherNo(int voucherno)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public InternalStockTransfer GetByVoucherNo(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool CheckIsVoucherNoExist(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public InternalStockTransfer GetByInvoiceNo(int bookno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookno && p.Id != id);
        }
        public bool CheckIdInvoiceNoExist(int bookno, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookno && p.Id != id);
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            //return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            var voucher = FiscalCollection
                                  .OrderByDescending(p => p.VoucherNumber)
                                  .FirstOrDefault();
            if (voucher == null)
                return maxnumber;

            return voucher.VoucherNumber + 1;
        }
        public override void Update(InternalStockTransfer r, bool received = false)
        {
            var dbInternalStockTransfer = GetById(r.Id,true);
            var ids = r.InternalStockTransferItems.Select(p => p.Id).ToList();
            var deletedIds = dbInternalStockTransfer.InternalStockTransferItems.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            new InternalStockTransferItemRepository(this).Delete(deletedIds);
            new InternalStockTransferItemRepository(this).Save(r.InternalStockTransferItems.ToList());
            r.InternalStockTransferItems = null;
            r.Status = received == true ? (byte)1 : dbInternalStockTransfer.Status;
            base.Update(r, true, false);
        }
        public InternalStockTransfer GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            InternalStockTransfer v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any())
            {
                v = new InternalStockTransfer();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
                v.CreatedAt = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                if (record != null)
                {
                    Db.InternalStockTransfers.Remove(record);
                    SaveLog(record, ActionType.Deleted);
                    Db.SaveChanges();
                }
                scope.Complete();
            }
        }
        public override void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = FiscalCollection.FirstOrDefault(p => p.Id == id);
                if (record != null)
                {
                    Db.InternalStockTransfers.Remove(record);
                    //string query = "Delete from OrderItems where OrderId='" + id + "'";
                    //Db.Database.ExecuteSqlCommand(query);
                    //record.IsDeleted = true;
                }
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public void Update(int srnno)
        {

            var srn = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == srnno);
            if (srn != null)
            {
                srn.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
            }
        }

    }
}
