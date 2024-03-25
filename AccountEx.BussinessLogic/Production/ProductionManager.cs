using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class ProductionManager
    {
        public static void Save(WorkInProgress p, bool addtrans, bool includOrder)
        {

            var repo = new WorkInProgressRepository();
            var transactionRepo = new TransactionRepository(repo);
            var orderBookingRepo = new OrderBookingRepository(repo);
            p.AccountId = SettingManager.FinishedProductionHeadId;
            if (p.VoucherType == VoucherType.None)
                p.VoucherType = VoucherType.Production;
            if (p.Id == 0)
            {
                p.CreatedDate = DateTime.Now;
                p.VoucherNumber = addtrans ? transactionRepo.GetNextVoucherNumber(p.VoucherType) : new WorkInProgressRepository().GetNextVoucherNumber(p.VoucherType);
                repo.Add(p, true, false);
            }
            else
            {
                repo.Update(p);
            }
            if (includOrder)
                orderBookingRepo.Update(p.OrderNo, VoucherType.SaleOrder, (byte)TransactionStatus.Ready);
            if (addtrans)
                AddTransaction(p, repo);
            repo.SaveChanges();
        }
        public static void SaveReceipe(ProductReceipe p)
        {
            var repo = new ProductReceipeRepository();
            if (p.Id == 0)
            {
                repo.Add(p, true, false);
            }
            else
            {
                repo.Update(p);
            }

            repo.SaveChanges();
        }

        public static string ValidateReceipeSave(ProductReceipe input)
        {
            var err = ",";
            try
            {
                var repo = new ProductReceipeRepository();
                foreach (var item in input.ProductReceipeitems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is not valid.,";
                }

                var Itemcountlist = input.ProductReceipeitems.GroupBy(p => p.ItemId).Select(p => new
                {
                    ItemId = p.Key,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    ItemName = p.FirstOrDefault().ItemName,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += item.ItemCode + "-" + item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                }

                var isExist = repo.IsReceipeExits(input.ProductId, input.Id);
                if (isExist)
                {
                    err += "Only one Receipe per product is allowed.,";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            err = err.Trim(',');
            return err;


        }

        public static void Delete(int voucherno, VoucherType transactiontype)
        {
            var transactionRepo = new TransactionRepository();
            var workInProgressRepo = new WorkInProgressRepository(transactionRepo);
            transactionRepo.HardDelete(voucherno, transactiontype);
            workInProgressRepo.DeleteByVoucherNumber(voucherno);
        }

        public static void AddTransaction(WorkInProgress p, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transactionRepo = new TransactionRepository(repo);
            transactionRepo.HardDelete(p.VoucherNumber, p.VoucherType);

            var trans = p.WPItems.Where(q => q.EntryType == (byte)EntryType.ServiceExpense).Select(item => new Transaction
            {
                AccountId = item.ItemId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.ServiceExpense,
                Date = p.Date,
                Credit = item.Amount,
                Comments = item.Comments

            }).ToList();

            trans.AddRange(p.WPItems.Where(q => q.EntryType == (byte)EntryType.ServiceExpense).Select(item => new Transaction
            {
                AccountId = SettingManager.WorkInProcessHeadId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.ServiceExpense,
                Date = p.Date,
                Debit = item.Amount,
                Comments = item.Comments

            }).ToList());




            trans.AddRange(p.WPItems.Where(q => q.EntryType == (byte)EntryType.RawMaterial).Select(item => new Transaction
            {
                AccountId = SettingManager.WorkInProcessHeadId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.RawMaterial,
                Date = p.Date,
                Debit = item.Amount,
                Comments = item.ItemName + " (" + item.Quantity + "X" + item.Rate + ")",

            }).ToList());

            trans.AddRange(p.WPItems.Where(q => q.EntryType == (byte)EntryType.RawMaterial).Select(item => new Transaction
            {
                AccountId = SettingManager.StockValueAccountId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.RawMaterial,
                Date = p.Date,
                Credit = item.Amount,
                Comments = item.ItemName + " (" + item.Quantity + "X" + item.Rate + ")",

            }).ToList());



            trans.AddRange(p.WPItems.Where(q => q.EntryType == (byte)EntryType.FinishedGoods).Select(item => new Transaction
            {
                AccountId = SettingManager.WorkInProcessHeadId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.FinishedGoods,
                Date = p.Date,
                Credit = item.Amount,
                Comments = item.ItemName + " (" + item.Quantity + "X" + item.Rate + ")",

            }).ToList());

            trans.AddRange(p.WPItems.Where(q => q.EntryType == (byte)EntryType.FinishedGoods).Select(item => new Transaction
            {
                AccountId = SettingManager.StockValueAccountId,
                Quantity = Numerics.GetInt(item.Quantity),
                Price = Numerics.GetDecimal(item.Rate),
                InvoiceNumber = p.InvoiceNumber,
                VoucherNumber = p.VoucherNumber,
                TransactionType = p.VoucherType,
                EntryType = (byte)EntryType.FinishedGoods,
                Date = p.Date,
                Debit = item.Amount,
                Comments = item.ItemName + " (" + item.Quantity + "X" + item.Rate + ")",

            }).ToList());





            //var voucherNumber = new TransactionRepository().GetNextVoucherNumber(s.TransactionType);
            p.VoucherNumber = p.VoucherNumber;
            foreach (var item in trans)
            {
                item.VoucherNumber = p.VoucherNumber;
                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transactionRepo.Add(trans);
        }
    }
}
