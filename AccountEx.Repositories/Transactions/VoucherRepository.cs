using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class VoucherRepository : GenericRepository<Transaction>
    {

        public new List<Transaction> GetById(int id)
        {
            var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
            var data = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
            return data;
        }
        public void Save(VoucherExtra obj)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var vou = new TransactionRepository().GetNextVoucherNumber(obj.Items[0].TransactionType);
                foreach (var item in obj.Items)
                {
                    item.VoucherNumber = vou;
                    item.CreatedDate = DateTime.Now;
                    item.CompanyId = SiteContext.Current.User.CompanyId;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                    Db.Transactions.Add(item);
                }
                SaveChanges();
                scope.Complete();
            }

        }

        public void Update(VoucherExtra obj)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var voucher = obj.Items.FirstOrDefault();
                if (voucher != null)
                {
                    var id = voucher.Id;
                    var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
                    var childItems = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType);
                    foreach (var item in obj.Items)
                    {
                        var child = childItems.FirstOrDefault(p => p.EntryType == item.EntryType);
                        if (child != null)
                        {
                            child.TransactionType = item.TransactionType;
                            child.EntryType = item.EntryType;
                            child.AccountId = item.AccountId;
                            child.AccountTitle = item.AccountTitle;
                            child.VoucherNumber = item.VoucherNumber;
                            child.Quantity = item.Quantity;
                            child.Comments = item.Comments;
                            child.Price = item.Price;
                            child.Debit = item.Debit;
                            child.Credit = item.Credit;
                            child.Date = item.Date;
                            child.FiscalId = SiteContext.Current.Fiscal.Id;
                        }
                    }
                }
                SaveChanges();
                scope.Complete();
            }

        }

        public override void Delete(int id)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {

                var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
                var child = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == masterdetail.VoucherNumber
                            && p.TransactionType == masterdetail.TransactionType && p.EntryType == (byte)EntryType.Item);

                if (child != null) Db.Transactions.Remove(child);
                if (masterdetail != null) Db.Transactions.Remove(masterdetail);

                scope.Complete();
            }
            SaveChanges();
        }


        public List<VoucherDetail> GetDetailedVouchers(DateTime d1, DateTime d2)
        {
            var list = new List<VoucherDetail>();
            //var vouchers = Db.Vouchers.Where (p=>! p.IsDeleted == null
            return list;
        }

        public dynamic GetPedningVouchers(VoucherType type, int supplierId)
        {
            var voucherQuery = AsQueryable<Voucher>(true);
            return voucherQuery.Where(p => !p.IsPaid && p.TransactionType == type && p.AccountId == supplierId).Select(p => new
            {
                p.VoucherNumber,
                p.BookNo,
                p.Id,
                p.NetTotal,
                p.TotalPaid
            }).ToList();

        }
        public dynamic GetVoucherByIdforPayablePayment(VoucherType type, int id)
        {
            var voucherQuery = AsQueryable<Voucher>(true);
            return voucherQuery.Where(p => p.Id == id && p.TransactionType == type).Select(p => new
            {
                p.VoucherNumber,
                p.BookNo,
                p.Id,
                p.NetTotal,
                p.TotalPaid
            }).FirstOrDefault();

        }


        public List<Voucher> GetByTypes(DateTime d1, DateTime d2, bool isDebit, out List<VoucherItem> vItems)
        {
            var voucherQuery = AsQueryable<Voucher>(true);
            var vouchers = voucherQuery.Where(p => p.Date >= d1 && p.Date <= d2);
            if (isDebit)
                vouchers = vouchers.Where(p => p.TransactionType  == VoucherType.TransferVoucher
                        || p.TransactionType  == VoucherType.CashPayments
                        || p.TransactionType  == VoucherType.BankPayments);
            else
                vouchers = vouchers.Where(p => p.TransactionType  == VoucherType.TransferVoucher
                        || p.TransactionType  == VoucherType.CashReceipts
                        || p.TransactionType  == VoucherType.BankReceipts);
            vouchers = vouchers.OrderBy(p => p.Date);
            var itemQuery = AsQueryable<Voucher>(true).SelectMany(p => p.VoucherItems);
            vItems = itemQuery.Join(vouchers, vi => vi.VoucherId, v => v.Id, (vi, v) => vi).ToList();
            return vouchers.ToList();
        }


    }
}