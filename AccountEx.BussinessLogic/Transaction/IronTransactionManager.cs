using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class IronTransactionManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new TransRepository();
                var transactionRepo = new TransactionRepository(repo);
                var dcRepo = new DeliveryChallanRepository(repo);

                sale.SaleItems.ToList().ForEach(x => { x.NetAmount = x.Amount; });
                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transactionRepo.GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale,true,false);
                    if (sale.TransactionType  == VoucherType.Sale && sale.DCNo > 0)
                    {
                        var dc = dcRepo.GetByVoucherNumber(sale.DCNo,
                           sale.TransactionType  == VoucherType.Sale || sale.TransactionType  == VoucherType.PurchaseReturn
                           ? VoucherType.GoodIssue
                            : sale.TransactionType  == VoucherType.Purchase ||
                            sale.TransactionType  == VoucherType.SaleReturn
                            ? VoucherType.GoodReceive
                            : VoucherType.None);
                        if (dc != null)
                        {
                            dc.Status = (byte)TransactionStatus.Dispatch;
                            dcRepo.Update(dc);
                        }
                    }
                }
                else
                {
                    repo.Update(sale);
                }
                AddTransaction(sale, sale.CashSale,repo);
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
                var transactionRepo=new TransactionRepository(saleRepo);
                transactionRepo.HardDelete(voucherno, transactiontype);
                saleRepo.DeleteByVoucherNumber(voucherno, transactiontype);
                scope.Complete();
            }
        }

        public static void AddTransaction(Sale s, bool isCashSale,BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transactionRepo=new TransactionRepository(repo);
            transactionRepo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>();
            //trans = s.SaleItems.Select(item => new Transaction
            //{
            //    AccountId = item.ItemId,
            //    Quantity = Numerics.GetInt(item.Quantity),
            //    Price = Numerics.GetDecimal(item.Rate),
            //    InvoiceNumber = s.InvoiceNumber,
            //    VoucherNumber = s.VoucherNumber,
            //    TransactionType = s.TransactionType,
            //    EntryType = (byte)EntryType.Item,

            //    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
            //            ? Numerics.GetInt(item.Amount)
            //            : 0,
            //    Debit =
            //        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
            //            ? Numerics.GetInt(item.Amount)
            //            : 0
            //}).ToList();

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
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = s.TransactionType  == VoucherType.Sale ? SettingManager.SaleAccountHeadId : s.TransactionType  == VoucherType.Purchase ? SettingManager.PurchaseAccountHeadId : s.TransactionType  == VoucherType.SaleReturn ? SettingManager.SaleReturnAccountHeadId : SettingManager.PurchaseReturnAccountHeadId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Quantity = 1,
                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.GrossTotal)
                        : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.GrossTotal)
                        : 0
            });
            //Add Transaction for All System Accounts
            //if (s.TransactionType  == VoucherType.Sale)
            //{
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = SettingManager.CuttingHeadId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Quantity = 1,
                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.Cutting)
                        : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.Cutting)
                        : 0
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = SettingManager.LoadingHeadId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Quantity = 1,
                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.Loading)
                        : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.Loading)
                        : 0
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = SettingManager.CarriageHeadId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Quantity = 1,
                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.Carriage)
                        : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.Carriage)
                        : 0
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = SettingManager.WhtacHeadId,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Quantity = 1,
                Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                        ? Numerics.GetDecimal(s.WHT)
                        : 0,
                Debit =
                    s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                        ? Numerics.GetDecimal(s.WHT)
                        : 0
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = s.InvoiceNumber,
                VoucherNumber = s.VoucherNumber,
                AccountId = SettingManager.GstacHeadId,
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
            // }

            if (Numerics.GetInt(s.Discount) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = AccountManager.GetLeafAccountId(Constants.AccountDiscount),
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetDecimal(s.Discount)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetDecimal(s.Discount)
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
            transactionRepo.Add(trans);
        }
    }
}
