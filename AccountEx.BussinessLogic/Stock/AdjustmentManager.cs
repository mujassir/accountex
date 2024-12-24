using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models.Transactions;

namespace AccountEx.BussinessLogic
{
    public static class AdjustmentManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new TransRepository();
                var transactionRepo = new TransactionRepository(repo);
                var vouchertype = new List<VoucherType>() { VoucherType.AdjustmentIn, VoucherType.AdjustmentOut };
                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transactionRepo.GetNextVoucherNumber(vouchertype);
                    repo.Add(sale, true, false);
                    //repo.SaveLog(sale, ActionType.Added);
                }
                else
                {
                    repo.Update(sale);
                    //repo.SaveLog(sale, ActionType.Updated);
                }
                AddTransaction(sale, sale.CashSale, repo);
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

        public static void Delete(int voucherno, VoucherType transactiontype)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var saleRepo = new SaleRepository();
                var transactionRepo = new TransactionRepository(saleRepo);

                transactionRepo.HardDelete(voucherno, transactiontype);
                saleRepo.DeleteByVoucherNumber(voucherno, transactiontype);
                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, bool isCashSale, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transactionRepo = new TransactionRepository(repo);
            transactionRepo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>
            {
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit =Numerics.GetInt(s.NetTotal),
                    Debit =0,

                   
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit =Numerics.GetInt(s.NetTotal),
                    Credit =0,

                   
                },
                //new Transaction
                //{
                //    InvoiceNumber = s.InvoiceNumber,
                //    VoucherNumber = s.VoucherNumber,
                //    AccountId = s.AccountId,
                //    TransactionType = s.TransactionType,
                //    EntryType = (byte) EntryType.MasterDetail,
                //    Quantity = 1,
                //    Credit =s.TransactionType == VoucherType.AdjustmentIn ? Numerics.GetInt(s.NetTotal): 0,
                //    Debit =s.TransactionType == VoucherType.AdjustmentOut ? Numerics.GetInt(s.NetTotal): 0,

                   
                //},
                //new Transaction
                //{
                //    InvoiceNumber = s.InvoiceNumber,
                //    VoucherNumber = s.VoucherNumber,
                //    AccountId =s.TransactionType == VoucherType.AdjustmentIn? SettingManager.PurchaseAccountHeadId:SettingManager.SaleAccountHeadId,
                //    TransactionType = s.TransactionType,   
                //    EntryType = (byte) EntryType.HeadAccount,
                //    Quantity = 1,
                //    Debit =s.TransactionType == VoucherType.AdjustmentIn ? Numerics.GetInt(s.NetTotal + s.Discount): 0,
                //    Credit =s.TransactionType == VoucherType.AdjustmentOut ? Numerics.GetInt(s.NetTotal + s.Discount): 0,
                   
                //}
            };
            //var voucherNumber = new TransactionRepository().GetNextVoucherNumber(s.TransactionType);
            s.VoucherNumber = s.VoucherNumber;
            foreach (var item in trans)
            {
                item.VoucherNumber = s.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = s.Date == DateTime.MinValue ? dt : s.Date;
                item.Comments = s.Comments;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transactionRepo.Add(trans);
        }

        public static void SaveDairyAdjustment(List<DairyAdjustment> items)
        {
            var dt = DateTime.Now;
            var repo = new GenericRepository<DairyAdjustment>();
            int voucherNumber = items.FirstOrDefault(x => x.VoucherNumber > 0)?.VoucherNumber 
                ?? (repo.GetAll().OrderByDescending(p => p.VoucherNumber).FirstOrDefault()?.VoucherNumber  ?? 0) + 1;

            var dbRecords = repo.GetAll(x => x.VoucherNumber == voucherNumber);
            foreach (var item in items)
            {
                var record = dbRecords.FirstOrDefault(x => x.Id == item.Id);
                if(record == null)
                {
                    item.VoucherNumber = voucherNumber;
                    item.CreatedAt = dt;
                    item.Date = item.Date == DateTime.MinValue ? dt : item.Date;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                    repo.Add(item);
                } else
                {
                    record.Date = item.Date == DateTime.MinValue ? dt : item.Date;
                    record.ItemId = item.ItemId;
                    record.ItemCode = item.ItemCode;
                    record.ItemName = item.ItemName;
                    record.Comment = item.Comment;
                    record.Milk = item.Milk;
                    record.ItemA = item.ItemA;
                    record.ItemB = item.ItemB;
                    record.ItemC = item.ItemC;
                    record.ItemD = item.ItemD;
                    record.Medicine = item.Medicine;
                    record.EntryType = item.EntryType;

                    record.ModifiedAt = dt;
                    record.FiscalId = SiteContext.Current.Fiscal.Id;
                    repo.Update(record);
                }
            }

            var removedRecords = dbRecords.Where(x => !items.Where(e => e.Id > 0).Select(e => e.Id).Contains(x.Id));
            foreach (var item in removedRecords)
                repo.Delete(item.Id);

            repo.SaveChanges();
        }

    }
}
