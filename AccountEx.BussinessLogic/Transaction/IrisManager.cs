using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class IrisManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var saleservicesitems = new List<SaleServicesItem>();
                var repo = new TransRepository();
               
                var saleServiceItemRepo = new SaleServicesItemRepository(repo);
                var serviceItemRepo = new  ServiceItemRepository(repo);
                var saleItemRepo = new SaleItemRepository(repo);
                var accountDetailRepo = new AccountDetailRepository(repo);
                var transRepo = new TransactionRepository(repo);
                var saleManIds = sale.SaleItems.Select(p => p.SalesmanId).ToList();

                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                if (sale.Id == 0)
                {
                    var salemans = accountDetailRepo.AsQueryable().Where(p => saleManIds.Contains(p.AccountId)).Select(p => new
                    {
                        p.AccountId,
                        p.CommissionPercent
                    }).ToList();
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent;
                        x.ComissionAmount = x.Amount * (salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent / 100);
                        x.Quantity = 1;
                        x.Rate = x.NetAmount = x.Amount;

                    });
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transRepo.GetNextVoucherNumber(sale.TransactionType);
                    //true is passed becouse we need to get SaleId to be used in child tables
                    repo.Add(sale,true,true);
                }
                else
                {

                    var prevcomission = saleItemRepo.GetBySaleId(sale.Id).ComissionPercent;
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = prevcomission;
                        x.ComissionAmount = x.Amount * (prevcomission / 100);
                        x.Quantity = 1;
                        x.Rate = x.NetAmount = x.Amount;

                    });
                    repo.Update(sale);
                }
                foreach (var saleItem in sale.SaleItems)
                {
                    var serviceitems = serviceItemRepo.GetByServiceId(saleItem.ItemId);
                    foreach (var serviceItem in serviceitems)
                    {

                        SaleServicesItem obj = new SaleServicesItem
                        {
                            SaleId = sale.Id,
                            SaleItemId = saleItem.Id,
                            ServiceId = saleItem.ItemId,
                            ServiceItemId = serviceItem.Id,
                            ServiceItemItemId = serviceItem.ItemId,
                            Quantity = Numerics.GetDecimal(serviceItem.Quantity)
                        };
                        saleservicesitems.Add(obj);
                    }
                    saleServiceItemRepo.Save(saleservicesitems, sale.Id);
                }
                AddTransaction(sale, sale.CashSale,repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }


        public static void FixData(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new TransRepository();
                var saleservicesitems = new List<SaleServicesItem>();
                var saleManIds = sale.SaleItems.Select(p => p.SalesmanId).ToList();

                //  sale.OtherAccountId = sale.CashSale ? SettingManager.CashAccountId : sale.TransactionType==VoucherType.Sale? SettingManager.SaleAccountHeadId;
                if (sale.Id == 0)
                {
                    var salemans = new AccountDetailRepository().AsQueryable().Where(p => saleManIds.Contains(p.AccountId)).Select(p => new
                    {
                        p.AccountId,
                        p.CommissionPercent
                    }).ToList();
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent;
                        x.ComissionAmount = x.Amount * (salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent / 100);
                        x.Quantity = 1;
                        x.Rate = x.NetAmount = x.Amount;

                    });
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = new TransactionRepository().GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
                    repo.SaveLog(sale, ActionType.Added);
                }
                else
                {

                    var prevcomission = new SaleItemRepository().GetBySaleId(sale.Id).ComissionPercent;
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = prevcomission;
                        x.ComissionAmount = x.Amount * (prevcomission / 100);
                        x.Quantity = 1;
                        x.Rate = x.NetAmount = x.Amount;

                    });
                    //var salemans = new AccountDetailRepository().AsQueryable().Where(p => saleManIds.Contains(p.AccountId)).Select(p => new
                    //{
                    //    p.AccountId,
                    //    p.CommissionPercent
                    //}).ToList();
                    //sale.SaleItems.ToList().ForEach(x =>
                    //{
                    //    x.ComissionPercent = salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent;
                    //    x.ComissionAmount = x.Amount * (salemans.FirstOrDefault(p => p.AccountId == x.SalesmanId).CommissionPercent / 100);
                    //    x.Quantity = 1;
                    //    x.Rate = x.NetAmount = x.Amount;

                    //});
                    repo.Update(sale);
                   // repo.SaveLog(sale, ActionType.Updated);
                }
                //foreach (var saleItem in sale.SaleItems)
                //{
                //    var serviceitems = new ServiceItemRepository().GetByServiceId(saleItem.ItemId);
                //    foreach (var serviceItem in serviceitems)
                //    {

                //        SaleServicesItem obj = new SaleServicesItem
                //        {
                //            SaleId = sale.Id,
                //            SaleItemId = saleItem.Id,
                //            ServiceId = saleItem.ItemId,
                //            ServiceItemId = serviceItem.Id,
                //            ServiceItemItemId = serviceItem.ItemId,
                //            Quantity = Numerics.GetDecimal(serviceItem.Quantity)
                //        };
                //        saleservicesitems.Add(obj);
                //    }
                //    new SaleServicesItemRepository().Save(saleservicesitems, sale.Id);
                //}
                AddTransaction(sale, sale.CashSale,repo);
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

        public static void AddTransaction(Sale s, bool isCashSale,BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var repo = new TransactionRepository(baseRepo);
            repo.HardDelete(s.VoucherNumber, s.TransactionType);
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
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
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
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = 
                    s.CashSale ? SettingManager.DailyRevenueHeadId : SettingManager.CreditCardSaleHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Debit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0
                },
                
            };
            ////// Start Of Commission Amount Transaction
            //Credit to Salesman
           // trans.AddRange(s.SaleItems.Select(item => new Transaction
           //{
           //    AccountId = item.SalesmanId,
           //    Quantity = Numerics.GetInt(item.Quantity),
           //    Price = Numerics.GetDecimal(item.Rate),
           //    InvoiceNumber = s.InvoiceNumber,
           //    VoucherNumber = s.VoucherNumber,
           //    TransactionType = s.TransactionType,
           //    EntryType = (byte)EntryType.Item,

           //    Credit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
           //            ? Numerics.GetInt(item.ComissionAmount)
           //            : 0,
           //    Debit =
           //        s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
           //            ? Numerics.GetInt(item.ComissionAmount)
           //            : 0
           //}).ToList());
           // //Debit to Comission Account
           // trans.AddRange(s.SaleItems.Select(item => new Transaction
           // {
           //     AccountId = SettingManager.ComissionAccountId,
           //     Quantity = Numerics.GetInt(item.Quantity),
           //     Price = Numerics.GetDecimal(item.Rate),
           //     InvoiceNumber = s.InvoiceNumber,
           //     VoucherNumber = s.VoucherNumber,
           //     TransactionType = s.TransactionType,
           //     EntryType = (byte)EntryType.Item,

           //     Debit = s.TransactionType  == VoucherType.Sale || s.TransactionType  == VoucherType.PurchaseReturn
           //             ? Numerics.GetInt(item.ComissionAmount)
           //             : 0,

           //     Credit = s.TransactionType  == VoucherType.SaleReturn || s.TransactionType  == VoucherType.Purchase
           //             ? Numerics.GetInt(item.ComissionAmount)
           //             : 0
           // }).ToList());
            ////// End Of Commission Amount Transaction

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
            repo.Add(trans);
        }
    }
}
