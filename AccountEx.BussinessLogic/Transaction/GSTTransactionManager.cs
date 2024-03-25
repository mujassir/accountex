using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class GstTransactionManager
    {


        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new TransRepository();
                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                NotificationActions action;
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = new TransactionRepository().GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
                    repo.SaveLog(sale, ActionType.Added);
                    action = NotificationActions.Add;
                }
                else
                {
                    repo.Update(sale);
                    repo.SaveLog(sale, ActionType.Updated);
                    action = NotificationActions.Modify;
                }
                AddTransaction(sale, sale.CashSale);
                //NotificationManager.AddTrigger(sale.Id,sale.TransactionType, action,repo);
                scope.Complete();
            }

        }

        public static Sale GetVocuherDetail(int voucherno, VoucherType transactiontype, string key)
        {
            bool next, previous;
            var d = new SaleRepository().GetByVoucherNumber(voucherno, transactiontype, key, out next, out previous);
            return d;
        }

        public static void Delete(int voucherno, VoucherType transactiontype)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                new TransactionRepository().HardDelete(voucherno, transactiontype);
                new SaleRepository().DeleteByVoucherNumber(voucherno, transactiontype);
                //NotificationManager.AddTrigger(0, transactiontype, NotificationActions.Delete);
                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, bool isCashSale)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = s.SaleItems.Select(item => new Transaction
            {
                AccountId = item.ItemId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.Item,

                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                    ? Numerics.GetDecimal(item.Amount)
                    : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(item.Amount)
                        : 0
            }).ToList();

            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = s.AccountId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.MasterDetail,

                Quantity = 1,
                Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.NetTotal)
                        : 0,
                Credit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.NetTotal)
                        : 0,

            });
            if (Numerics.GetInt(s.GST) > 0)
            {

                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.HeadAccount,
                    Quantity = 1,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetDecimal(s.GST)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetDecimal(s.GST)
                            : 0
                });

            }
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
            new TransactionRepository().Add(trans);
        }
    }
}
