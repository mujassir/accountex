using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using System.Collections.Generic;

namespace AccountEx.BussinessLogic
{
    public static class WheatPurchaseManager
    {


        public static void Save(WheatPurchase wp)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new WheatPurchaseRepository();
                //repo.DeleteByVoucherNumber(voucherno);
                repo.Save(wp);
                repo.SaveLog(wp, ActionType.Added);
                wp.WheatPurchaseItems = wp.WheatPurchaseItems.Where(p => p.Amount > 0).ToList();
                AddTransaction(wp, repo);
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
                new WheatPurchaseRepository().DeleteByVoucherNumber(voucherno);
                scope.Complete();
            }

        }

        public static void AddTransaction(WheatPurchase wp,BaseRepository repo)
        {
            var dt = DateTime.Now;
            var voucherno = wp.VoucherNumber;
            var tranRepo = new TransactionRepository(repo);
            tranRepo.HardDelete(voucherno, VoucherType.WheatPurchase);
            var trans = new List<Transaction>();
            if (!wp.IsGovt)
            {
                //Credit  Amount to Parties
                trans = wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = item.PartyId,
                    Quantity = 1,
                    Price = item.Amount,
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = item.Amount

                }).ToList();
                var a = 20;
                //var weingAmount = Constants.WeingAmount;
                //var weighBridgeAmount = Constants.WeighBridgeAmount;
                //var wheatTradeAmount = Constants.WheatTradeAmount;
                //if (wp.NetWeightTotal > 30000)
                //{
                //    weingAmount = 190;
                //    weighBridgeAmount = 164;
                //    wheatTradeAmount = 26;
                //}
                //Debit  Amount to Wheat Account(Fixed)
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = SettingManager.WheatHeadId,
                    Quantity = 1,
                    Price = item.Amount - Numerics.GetDecimal(item.BardanaRate * item.Bags),
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = item.Amount + ((item.NetWeight > 30000 ? SettingManager.WheatPurcaseWeighBridgeAmount1 : SettingManager.WheatPurcaseWeighBridgeAmount)+SettingManager.WheatTradeAmount) - Numerics.GetDecimal(item.BardanaRate * item.Bags)

                }).ToList());
                //Credit 126 rupee to Weight Brige Account(Fixed)
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = SettingManager.WeighBridgeHeadId,
                    Quantity = 1,
                    Price = (item.NetWeight > 30000 ? SettingManager.WheatPurcaseWeighBridgeAmount1 : SettingManager.WheatPurcaseWeighBridgeAmount),
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = (item.NetWeight > 30000 ? SettingManager.WheatPurcaseWeighBridgeAmount1 : SettingManager.WheatPurcaseWeighBridgeAmount)

                }).ToList());
                //Credit 44(170-126=44) Rupee to WheatTrade Account(Fixed)
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = SettingManager.WheatTradeHeadId,
                    Quantity = 1,
                    Price = SettingManager.WheatTradeAmount,
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = SettingManager.WheatTradeAmount

                }).ToList());

                //Debit Bardana Account if Bradana Rate exits
                trans.AddRange(wp.WheatPurchaseItems.Where(p => p.BardanaRate > 0).Select(item => new Transaction
                {
                    AccountId = item.BardanaType == (byte)BardanaType.Plastic ? SettingManager.BardanaPlasticHeadId : SettingManager.BardanaJuteHeadId,
                    Quantity = 1,
                    Price = Numerics.GetDecimal(item.BardanaRate * item.Bags),
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = Numerics.GetDecimal(item.BardanaRate * item.Bags)

                }).ToList());
            }

            else
            {
                //Credit  Amount to Food Department
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = SettingManager.FoodDepartmentHeadId,
                    Quantity = 1,
                    Price = item.Amount,
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = item.Amount

                }).ToList());
                //Debit  Amount to Government Wheat
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = SettingManager.GovtWheatHeadId,
                    Quantity = 1,
                    Price = item.Amount - Numerics.GetDecimal(item.BardanaRate * item.Bags),
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = item.Amount - Numerics.GetDecimal(item.BardanaRate * item.Bags)

                }).ToList());

                //Debit  Amount to  Govenment Plastic or Govt Jute
                trans.AddRange(wp.WheatPurchaseItems.Select(item => new Transaction
                {
                    AccountId = item.BardanaType == (byte)BardanaType.Plastic ? SettingManager.GovtPlasticHeadId : SettingManager.GovtJuteHeadId,
                    Quantity = 1,
                    Price = Numerics.GetDecimal(item.BardanaRate * item.Bags),
                    InvoiceNumber = voucherno,
                    VoucherNumber = voucherno,
                    TransactionType = VoucherType.WheatPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = Numerics.GetDecimal(item.BardanaRate * item.Bags)

                }).ToList());
            }
            foreach (var item in trans)
            {
                item.VoucherNumber = voucherno;
                item.CreatedDate = dt;
                item.Date = wp.Date == DateTime.MinValue ? dt : wp.Date;
                item.Comments = "";
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            tranRepo.Add(trans);
        }
    }
}
