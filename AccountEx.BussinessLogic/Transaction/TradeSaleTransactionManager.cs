using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class TradeSaleTransactionManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new SaleRepository();
                var transactionRepo=new TransactionRepository(repo);
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transactionRepo.GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
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

                new TransactionRepository().HardDelete(voucherno, transactiontype);
                new SaleRepository().DeleteByVoucherNumber(voucherno, transactiontype);

                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, bool isCashSale,BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transactionRepo=new TransactionRepository(repo);
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
                    Debit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.GrossTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.GrossTotal)
                            : 0,
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == VoucherType.Sale
                            ? SettingManager.SaleAccountHeadId
                            : s.TransactionType == VoucherType.Purchase
                                ? SettingManager.PurchaseAccountHeadId
                                : s.TransactionType == VoucherType.SaleReturn
                                    ? SettingManager.SaleReturnAccountHeadId
                                    : SettingManager.PurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.GrossTotal)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.GrossTotal)
                            : 0
                }
            };
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




            if (Numerics.GetInt(s.Discount) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.DiscountAccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.Discount)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.Discount)
                            : 0
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.Discount)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.Discount)
                            : 0
                });


            }

            if (Numerics.GetInt(s.AdvanceTaxTotal) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.AdvanceTaxacHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.AdvanceTaxTotal)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.AdvanceTaxTotal)
                            : 0
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.AdvanceTaxTotal)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.AdvanceTaxTotal)
                            : 0
                });


            }


            if (Numerics.GetInt(s.PromotionTotal) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Promotion,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.PromotionTotal)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.PromotionTotal)
                            : 0
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.PromotionAcccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Promotion,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.PromotionTotal)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.PromotionTotal)
                            : 0
                });


            }

            if (s.TransactionType  == VoucherType.Sale)
            {
                var saleman = new AccountDetailRepository().GetByAccountId(s.SalemanId.Value);
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = saleman.CollectionAccountId.Value,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Promotion,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Promotion,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0
                });


            }
            if (Numerics.GetInt(s.TotalFreight) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.VehicleId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Debit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.FreightHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Credit =
                        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
                            ? Numerics.GetInt(s.TotalFreight)
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
        public static string ValidateSave(Sale input)
        {
            var err = TransactionManager.ValidateSave(input);
            if (!input.SalemanId.HasValue)
            {
                err += "salesman is required.,";

            }
            else
            {
                var saleman = new AccountDetailRepository().GetByAccountId(input.SalemanId.Value);
                if (!saleman.CollectionAccountId.HasValue)
                {
                    err += "No collection account is linked with salesman.,";

                }
            }
            if (!input.OrderTakerId.HasValue)
            {
                err += "Order taker is required.,";
            }
            if (!input.TerritoryManagerId.HasValue)
            {
                err += "Territory ManagerId is required.,";
            }
            return err.TrimEnd(',');
        }

    }
}
