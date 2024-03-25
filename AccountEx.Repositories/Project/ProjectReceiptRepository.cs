using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class ProjectReceiptRepository : GenericRepository<ProjectReceipt>
    {
        public List<int> GetVoucherNumbers(int projectId)
        {
            return FiscalCollection.Where(p => p.ProjectId == projectId).Select(p => p.VoucherNumber).ToList();
        }
        public List<int> GetVoucherIds(int projectId)
        {
            return FiscalCollection.Where(p => p.ProjectId == projectId).Select(p => p.BankReceiptId).ToList();
        }

        //public List<dynamic> GetVoucher(int projectId)
        //{
        //    var voucherNumbers=GetVoucherNumbers(projectId);
        //    var vouchers =AsQueryable<Voucher>(true);
        //    vouchers = vouchers.Where(p => p.TransactionType  == VoucherType.BankReceipts && voucherNumbers.Contains(p.VoucherNumber));

        //    var records = AsQueryable<VoucherItem>().Join(vouchers, vi => vi.VoucherId, v => v.Id, (vi, v) => new { VoucherItem = vi, Voucher = v })
        //        .Where(p=>p.VoucherItem.Credit>0).
        //        .Select(x =>x new DataTableProjectReceiptExtra()
        //       {
        //           VoucherNumber = x.Voucher.VoucherNumber,
        //           Amount=0,
        //           AccountCode = x.VoucherItem.,
        //           AccountName = x.FirstOrDefault().Order.AccountName,
        //           Date = x.FirstOrDefault().Order.Date,

        //       }).AsQueryable();
        //}

        public bool AddReceipt(ProjectReceipt entity)
        {
            var payment = AsQueryable<Voucher>(true).FirstOrDefault(p => p.VoucherNumber == entity.VoucherNumber && p.TransactionType == entity.TransactionType);
            if (payment == null) return false;
            //var project = Db.Projects.FirstOrDefault(p => p.Id == entity.ProjectId);
            //project.Balance = project.Balance - receipt.Debit - receipt.Credit;
            entity.BankReceiptId = payment.Id;
            Add(entity);
            return true;
        }
        public void DeleteByVoucher(int voucherNumber)
        {
            var pr = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherNumber);
            if (pr != null)
            {
                Db.ProjectReceipts.Remove(pr);
                SaveChanges();
            }
        }
        public decimal GetReceivings(int projectId)
        {
            var receipts = FiscalCollection.Where(p => p.ProjectId == projectId).Select(p => p.VoucherNumber).ToList();
            var query = Db.Transactions.Where(p => receipts.Contains(p.VoucherNumber) && p.TransactionType == VoucherType.Salary);
            return query.Any() ? query.Sum(p => p.Debit + p.Credit) : 0;
        }
        public decimal GetReceivings1(int projectId)
        {
            var receipts = FiscalCollection.Where(p => p.ProjectId == projectId).Select(p => p.VoucherNumber).ToList();
            var query = Db.Transactions.Where(p => receipts.Contains(p.VoucherNumber) && p.TransactionType == VoucherType.BankReceipts);
            return query.Any() ? query.Sum(p => p.Debit + p.Credit) : 0;
        }
        public decimal GetPayments1(int projectId)
        {
            var voucherIds = FiscalCollection.Where(p => p.ProjectId == projectId).Select(p => p.BankReceiptId).ToList();
            var query = AsQueryable<VoucherItem>().Where(p => voucherIds.Contains(p.VoucherId) && p.Debit > 0);
            return query.Any() ? query.Sum(p => p.Debit.Value) : 0;
        }
    }
}
