using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.Repositories;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class CGSManager
    {


        public static List<Transaction> GetTransaction(Sale voucher, BaseRepository repo)
        {
            var trans = new List<Transaction>();
            if (SettingManager.AllowCGS && (voucher.TransactionType == VoucherType.Sale || voucher.TransactionType == VoucherType.SaleReturn))
            {
                var items = new List<SaleItem>();
                //if (s.TransactionType == VoucherType.Sale)
                //{

                var balances = new TransactionRepository(repo).GetStockAvgRates(voucher.SaleItems.Select(p => p.ItemId).Distinct().ToList(), voucher.Date);
                items = voucher.SaleItems.CloneWithJson().ToList();
                foreach (var item in items)
                {
                    var avgRate = balances.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (avgRate != null)
                    {
                        if (avgRate.Rate <= 0)
                        {
                            throw new OwnException("Please update rate for Item '" + item.ItemName + "' in Opening balance form.");
                        }
                        item.Rate = avgRate.Rate;
                        item.Amount = item.Quantity * item.Rate;
                        item.Comment = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")";
                    }
                }
                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.StockValueAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    ProductId = item.ItemId,
                    InvoiceNumber = voucher.InvoiceNumber,
                    VoucherNumber = voucher.VoucherNumber,
                    TransactionType = voucher.TransactionType,
                    EntryType = (byte)EntryType.CGS,
                    Credit = voucher.TransactionType == VoucherType.Sale ? item.Amount : 0,
                    Debit = voucher.TransactionType == VoucherType.SaleReturn ? item.Amount : 0,
                    Comments = item.Comment


                }).ToList());

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.CGSAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    ProductId = item.ItemId,
                    InvoiceNumber = voucher.InvoiceNumber,
                    VoucherNumber = voucher.VoucherNumber,
                    TransactionType = voucher.TransactionType,
                    EntryType = (byte)EntryType.CGS,
                    Debit = voucher.TransactionType == VoucherType.Sale ? item.Amount : 0,
                    Credit = voucher.TransactionType == VoucherType.SaleReturn ? item.Amount : 0,
                    Comments = item.Comment

                }).ToList());


            }
            return trans;
        }


        public static string IsSaleExitAfterPurchase(Sale voucher, SaleRepository saleRepo)
        {
            var err = "";
            if (SettingManager.AllowCGS && voucher.TransactionType == VoucherType.Purchase)
            {

                var dbSaleItems = saleRepo.IsSaleExitAfterPurchase(voucher.Id, voucher);
                foreach (var item in dbSaleItems)
                {
                    err += item.Name + " has sale and add/edit/delete will affect inventory.,";
                }
            }
            return err;
        }

        public static string IsSaleExitAfterPurchase(int voucherno, VoucherType transactiontype,SaleRepository saleRepo)
        {
            var err = "";
            if (SettingManager.AllowCGS && transactiontype == VoucherType.Purchase)
            {
                var purchaseId = saleRepo.GetId(voucherno, transactiontype);
                var dbSaleItems = saleRepo.IsSaleExitAfterPurchase(purchaseId);
                foreach (var item in dbSaleItems)
                {
                    err += item.Name + " has sale and delete will affect inventory.";
                }
            }
            return err;
        }



    }
}
