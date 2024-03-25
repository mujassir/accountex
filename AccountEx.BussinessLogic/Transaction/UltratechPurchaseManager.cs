using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class UltratechPurchaseManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var purchase = sale;
                var repo = new SaleRepository();
                var transRepo = new TransactionRepository(repo);

                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transRepo.GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
                    if (SiteContext.Current.User.CompanyId == 36)
                        AddKanTransaction(sale, repo);
                    else
                        AddTransaction(sale, repo);
                }
                else
                {
                    if (SiteContext.Current.User.CompanyId == 36)
                        AddKanTransaction(sale, repo);
                    else
                        AddTransaction(sale, repo);
                    repo.Update(sale);
                }
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
                var repo = new SaleRepository();
                var transRepo = new TransactionRepository(repo);
                transRepo.HardDelete(voucherno, transactiontype);
                repo.DeleteByVoucherNumber(voucherno, transactiontype);
                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            transRepo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>();

            //Credit  Amount to Parties
            trans = s.SaleItems.Select(item => new Transaction
            {
                AccountId = s.AccountId,
                Quantity = 1,
                Price = item.Amount,
                InvoiceNumber = item.VoucherNumber,
                VoucherNumber = item.VoucherNumber,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.MasterDetail,
                Credit = s.TransactionType  == VoucherType.GstPurchase ? item.Amount : 0,
                Debit = s.TransactionType  == VoucherType.GstPurchaseReturn ? item.Amount : 0,

            }).ToList();


            //Credit  Amount to Duties Exp 
            trans.AddRange(s.SaleItems.Select(item => new Transaction
            {
                AccountId = SettingManager.DutiesExpenseHeadId,
                Quantity = 1,
                Price = Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others),
                InvoiceNumber = item.VoucherNumber,
                VoucherNumber = item.VoucherNumber,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Credit = s.TransactionType  == VoucherType.GstPurchase ? Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others) : 0,
                Debit = s.TransactionType  == VoucherType.GstPurchaseReturn ? Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others) : 0
            }).ToList());

            ////Credit  Amount to CD
            //trans.AddRange(s.SaleItems.Where(p => p.CD > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.CDHeadId,
            //    Quantity = 1,
            //    Price = item.CD,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.CD

            //}).ToList());



            ////Credit  Amount to RD
            //trans.AddRange(s.SaleItems.Where(p => p.RD > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.RDHeadId,
            //    Quantity = 1,
            //    Price = item.RD,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.RD

            //}).ToList());

            ////Credit  Amount to SED
            //trans.AddRange(s.SaleItems.Where(p => p.SED > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.SEDHeadId,
            //    Quantity = 1,
            //    Price = item.SED,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.SED

            //}).ToList());

            ////Credit  Amount to Freight
            //trans.AddRange(s.SaleItems.Where(p => p.Freight > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.FreightHeadId,
            //    Quantity = 1,
            //    Price = item.Freight,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.Freight

            //}).ToList());

            ////Credit  Amount to FWD
            //trans.AddRange(s.SaleItems.Where(p => p.FWD > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.FWDHeadId,
            //    Quantity = 1,
            //    Price = item.FWD,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.FWD

            //}).ToList());

            ////Credit  Amount to Others
            //trans.AddRange(s.SaleItems.Where(p => p.Others > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.OthersHeadId,
            //    Quantity = 1,
            //    Price = item.Others,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.Others

            //}).ToList());


            ////Credit  to Gst
            //trans.AddRange(s.SaleItems.Where(p => p.GSTAmount > 0).Select(item => new Transaction
            //{
            //    AccountId = SettingManager.GstHeadId,
            //    Quantity = 1,
            //    Price = item.GSTAmount,
            //    InvoiceNumber = item.VoucherNumber,
            //    VoucherNumber = item.VoucherNumber,
            //    TransactionType = VoucherType.Purchase,
            //    EntryType = (byte)EntryType.Item,
            //    Credit = item.GSTAmount

            //}).ToList());

            //Debit  to Gst
            trans.AddRange(s.SaleItems.Where(p => p.GSTAmount > 0).Select(item => new Transaction
            {
                AccountId = SettingManager.GstHeadId,
                Quantity = 1,
                Price = item.GSTAmount,
                InvoiceNumber = item.VoucherNumber,
                VoucherNumber = item.VoucherNumber,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.Item,
                Debit = s.TransactionType  == VoucherType.GstPurchase ? item.GSTAmount : 0,
                Credit = s.TransactionType  == VoucherType.GstPurchaseReturn ? item.GSTAmount : 0,


            }).ToList());


            //Debit  NetTotal to Purchase Account
            trans.AddRange(s.SaleItems.Select(item => new Transaction
            {
                AccountId = s.TransactionType  == VoucherType.GstPurchase ? SettingManager.GstPurchaseAccountHeadId
                : SettingManager.GstPurchaseReturnAccountHeadId,
                Quantity = 1,
                Price = item.NetAmount + item.GSTAmount,
                InvoiceNumber = item.VoucherNumber,
                VoucherNumber = item.VoucherNumber,
                TransactionType = s.TransactionType,
                EntryType = (byte)EntryType.Item,
                Debit = s.TransactionType  == VoucherType.GstPurchase ? item.NetAmount : 0,
                Credit = s.TransactionType  == VoucherType.GstPurchaseReturn ? item.NetAmount : 0,

            }).ToList());

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
            transRepo.Add(trans);
        }

        //Transaction for Kan Energy Only
        public static void AddKanTransaction(Sale s, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            transRepo.HardDelete(s.VoucherNumber, s.TransactionType);




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
                    Credit = s.TransactionType  == VoucherType.GstPurchase ? s.NetTotal : 0,
                    Debit = s.TransactionType  == VoucherType.GstPurchaseReturn ? s.NetTotal : 0,
                },

                 new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.TransactionType  == VoucherType.GstPurchase? SettingManager.GstPurchaseAccountHeadId
                    :SettingManager.GstPurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit = s.TransactionType  == VoucherType.GstPurchase ? s.GrossTotal : 0,
                    Credit = s.TransactionType  == VoucherType.GstPurchaseReturn ? s.GrossTotal : 0,
                },
                 new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit = s.TransactionType  == VoucherType.GstPurchase ? s.GstAmountTotal : 0,
                    Credit  = s.TransactionType  == VoucherType.GstPurchaseReturn ? s.GstAmountTotal : 0,
                },
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
            transRepo.Add(trans);
        }
    }
}
