using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class ServiceOrderManager
    {
        public static void Save(Order order)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new OrderBookingRepository();
                if (order.Id == 0)
                {
                    order.Status = (byte)TransactionStatus.Pending;
                    repo.Add(order, true, false);
                }
                else
                {
                    repo.Update(order);
                }
                repo.SaveChanges();
                MarkFinal(order);
                scope.Complete();
            }

        }
        public static byte MarkFinal(int orderid)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new OrderBookingRepository();
                var record = repo.GetById(orderid);
                var r = MarkFinal(record);
                scope.Complete();
                repo.SaveChanges();
                return r;

            }


        }
        public static byte MarkFinal(Order record)
        {
            //using (var scope = TransactionScopeBuilder.Create())
            //{
            var repo = new OrderBookingRepository();
            var transRepo = new TransactionRepository(repo);
            //record.Status = (byte)TransactionStatus.Delivered;
            //repo.Update(record.Id, (byte)TransactionStatus.Delivered);
            transRepo.HardDelete(record.VoucherNumber, record.TransactionType);
            ServiceOrderManager.AddTransaction(record, transRepo);


            ///This code will be used for final and unfinal
            //if (record.Status == (byte)TransactionStatus.Delivered)
            //{
            //    record.Status = (byte)TransactionStatus.Pending;
            //    repo.Update(record.Id, (byte)TransactionStatus.Pending);
            //    transRepo.HardDelete(record.VoucherNumber, record.TransactionType);

            //}
            //else
            //{
            //    record.Status = (byte)TransactionStatus.Delivered;
            //    repo.Update(record.Id, (byte)TransactionStatus.Delivered);
            //    ServiceOrderManager.AddTransaction(record, transRepo);
            //}
            repo.SaveChanges();
            //scope.Complete();
            return record.Status;
            //}


        }
        public static void Delete(int voucherno, VoucherType transactiontype, int id)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                new SaleServicesItemRepository().DeleteBySaleId(id);
                new ServiceExpensesRepository().DeleteBySaleId(id);
                new TransactionRepository().HardDelete(voucherno, transactiontype);
                new SaleRepository().DeleteByVoucherNumber(voucherno, transactiontype);

                scope.Complete();
            }

        }
        public static void AddTransaction(Order o, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(o.VoucherNumber, o.TransactionType);
            var trans = new List<Transaction>();
            var gstAmount = o.OrderExpenseItems.Sum(p => p.GSTAmount);





            //Credit Amount to each selected service/employee
            trans.AddRange(o.OrderExpenseItems.Select(item => new Transaction
            {
                ReferenceId = o.Id,
                InvoiceNumber = o.InvoiceNumber,
                VoucherNumber = o.VoucherNumber,
                AccountId = item.EmployeeId,
                TransactionType = o.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Credit = Numerics.GetInt(item.Amount),
                Date = item.Date == DateTime.MinValue ? dt : item.Date,
                Comments = "Expenses against job no " + o.InvoiceNumber



            }));
            //Debit Amount to each selected expense
            trans.AddRange(o.OrderExpenseItems.Select(item => new Transaction
            {
                ReferenceId = o.Id,
                InvoiceNumber = o.InvoiceNumber,
                VoucherNumber = o.VoucherNumber,
                AccountId = Numerics.GetInt(item.ExpenseId),
                TransactionType = o.TransactionType,
                EntryType = (byte)EntryType.HeadAccount,
                Debit = Numerics.GetInt(item.Amount),
                Date = item.Date == DateTime.MinValue ? dt : item.Date,
                Comments = "Expenses against job no " + o.InvoiceNumber


            }));
            if (o.TransactionType  == VoucherType.CustomerServiceOrder)
            {
                //var services = o.OrderItems.Where(p => p.EntryType == (byte)EntryType.Services).ToList();
                ////Debit Service Amount to Customer
                //trans.AddRange(services.Select(item => new Transaction
                //{
                //    ReferenceId = o.Id,
                //    InvoiceNumber = o.InvoiceNumber,
                //    VoucherNumber = o.VoucherNumber,
                //    AccountId = o.AccountId,
                //    TransactionType = o.TransactionType,
                //    EntryType = (byte)EntryType.Services,
                //    Debit = Numerics.GetInt(item.Amount),


                //}));
                ////Debit Amount to each selected expense
                //trans.AddRange(services.Select(item => new Transaction
                //{
                //    ReferenceId = o.Id,
                //    InvoiceNumber = o.InvoiceNumber,
                //    VoucherNumber = o.VoucherNumber,
                //    AccountId = Numerics.GetInt(item.ItemId),
                //    TransactionType = o.TransactionType,
                //    EntryType = (byte)EntryType.Services,
                //    Credit = Numerics.GetInt(item.Amount),
                //}));

            }


            if (SettingManager.AllowCGS && (o.TransactionType  == VoucherType.SiteServiceOrder || o.TransactionType  == VoucherType.RepairingServiceOrder))
            {
                var balances = new TransactionRepository().GetStockAvgRates(o.OrderItems.Select(p => p.ItemId).Distinct().ToList(),o.Date);
                var items = o.OrderItems.CloneWithJson();
                foreach (var item in items)
                {
                    var t = balances.FirstOrDefault(p => p.ItemId == item.ItemId);
                    if (t != null)
                    {
                        item.Rate = t.Rate;
                        item.Amount = item.Quantity * item.Rate;

                    }
                }

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.StockConsumptionAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = o.InvoiceNumber,
                    VoucherNumber = o.VoucherNumber,
                    TransactionType = o.TransactionType,
                    EntryType = (byte)EntryType.InventoryConsumpation,
                    Credit = item.Amount,
                    Comments = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")",
                    Date = !item.Date.HasValue ? dt : item.Date.Value,



                }).ToList());

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = o.TransactionType  == VoucherType.SiteServiceOrder ? o.AccountId : o.MachineId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = o.InvoiceNumber,
                    VoucherNumber = o.VoucherNumber,
                    TransactionType = o.TransactionType,
                    EntryType = (byte)EntryType.InventoryConsumpation,
                    Debit = item.Amount,
                    Comments = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")",
                    Date = !item.Date.HasValue ? dt : item.Date.Value,
                }).ToList());


            }



            foreach (var item in trans)
            {
                item.VoucherNumber = o.VoucherNumber;
                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans, true);
        }

    }
}
