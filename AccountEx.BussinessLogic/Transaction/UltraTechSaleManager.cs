using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AccountEx.BussinessLogic
{
    public static class UltraTechSaleManager
    {


        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new SaleRepository();
                var transRepo = new TransactionRepository(repo);
                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                NotificationActions action;
                if (sale.Id == 0)
                {
                    if (sale.TransactionType  == VoucherType.GstSale)
                    {

                        //var commissionPercent = new AccountDetailRepository().AsQueryable().FirstOrDefault(p => p.AccountId == sale.SalemanId).CommissionPercent;

                        //    sale.ComissionPercent = commissionPercent;
                        //    sale.Comments
                    }
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transRepo.GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
                    action = NotificationActions.Add;
                }
                else
                {
                    //if (sale.TransactionType  == VoucherType.GstSale)
                    //{
                    //var prevcomission = new SaleItemRepository().GetBySaleId(sale.Id).ComissionPercent;
                    //sale.SaleItems.ToList().ForEach(x =>
                    //{
                    //    x.ComissionPercent = prevcomission;
                    //    x.ComissionAmount = x.Amount * (prevcomission / 100);

                    //});


                    //}
                    repo.Update(sale);
                    action = NotificationActions.Modify;
                }
                AddTransaction(sale, sale.CashSale, repo);

                //NotificationManager.AddTrigger(sale.Id, sale.TransactionType, action, repo);
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
                //NotificationManager.AddTrigger(0, transactiontype, NotificationActions.Delete);
                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, bool isCashSale, BaseRepository repo)
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
                    Debit =
                        s.TransactionType == VoucherType.GstSale ||
                        s.TransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetDecimal(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetDecimal(s.NetTotal)
                            : 0,
                            Comments=s.Comments
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == VoucherType.GstSale
                            ? SettingManager.GstSaleAccountHeadId
                            : s.TransactionType == VoucherType.GstPurchase
                                ? SettingManager.GstPurchaseAccountHeadId
                                : s.TransactionType == VoucherType.GstSaleReturn
                                    ? SettingManager.GstSaleReturnAccountHeadId
                                    : SettingManager.GstPurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit =
                        s.TransactionType == VoucherType.GstSale ||
                        s.TransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetDecimal(s.NetTotal - s.GstAmountTotal)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetDecimal(s.NetTotal - s.GstAmountTotal)
                            : 0,
                              Comments=s.Comments
                }
            };
            if (Numerics.GetDecimal(s.GstAmountTotal) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Credit = s.TransactionType  == VoucherType.GstSale || s.TransactionType  == VoucherType.GstPurchaseReturn
                  ? Numerics.GetDecimal(s.GstAmountTotal)
                  : 0,
                    Debit =
                        s.TransactionType  == VoucherType.GstSaleReturn || s.TransactionType  == VoucherType.GstPurchase
                            ? Numerics.GetDecimal(s.GstAmountTotal)
                            : 0,
                    Comments = s.Comments

                });
            }

            if (SettingManager.AllowCGS)
            {
                var balances = new TransactionRepository().GetStockAvgRates(s.SaleItems.Select(p => p.ItemId).Distinct().ToList(),s.Date);
                var items = s.SaleItems.CloneWithJson();
                foreach (var item in items)
                {
                    var t = balances.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (t != null)
                    {
                        item.Rate = t.Rate;
                        item.Amount = item.Quantity * item.Rate;
                        item.Comment = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")";
                    }
                }

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.StockValueAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.CGS,
                    Credit = s.TransactionType  == VoucherType.GstSale ? item.Amount : 0,
                    Debit = s.TransactionType  == VoucherType.GstSaleReturn ? item.Amount : 0,
                    Comments=item.Comment


                }).ToList());

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.CGSAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.CGS,
                    Debit = s.TransactionType  == VoucherType.GstSale ? item.Amount : 0,
                    Credit = s.TransactionType  == VoucherType.GstSaleReturn ? item.Amount : 0,
                    Comments = item.Comment

                }).ToList());


            }

            //var voucherNumber = new TransactionRepository().GetNextVoucherNumber(s.TransactionType);
            s.VoucherNumber = s.VoucherNumber;
            foreach (var item in trans)
            {
                item.VoucherNumber = s.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = s.Date == DateTime.MinValue ? dt : s.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans);
        }
    }
}
