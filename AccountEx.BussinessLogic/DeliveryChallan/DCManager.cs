using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class DCManager
    {


        public static void Save(DeliveryChallan dc, bool addTransaction)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new DeliveryChallanRepository();
                if (dc.Id == 0)
                {
                    dc.CreatedDate = DateTime.Now;
                    dc.VoucherNumber = repo.GetNextVoucherNumber(dc.TransactionType);
                    repo.Save(dc);
                }
                else
                {
                    repo.Update(dc, repo);
                }
                repo.SaveChanges();
                scope.Complete();
            }

        }

        public static void SaveGINP(DeliveryChallan dc, bool addTransaction)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new GINPRepository();
                var voucherType = new List<VoucherType>();
                /////code for serviceorder addition
                if (dc.TransactionType == VoucherType.GINP)
                {
                    voucherType.Add(VoucherType.GINP);
                }
                else
                {
                    voucherType.Add(VoucherType.CustomerServiceOrder);
                    voucherType.Add(VoucherType.SiteServiceOrder);
                    voucherType.Add(VoucherType.RepairingServiceOrder);
                }
                if (dc.Id == 0)
                {
                    dc.CreatedDate = DateTime.Now;
                    dc.VoucherNumber = repo.GetNextVoucherNumber(voucherType);
                    repo.Save(dc);
                }
                else
                {
                    repo.Update(dc, repo);
                }
                if (addTransaction)
                {

                    AddTransaction(dc, repo);

                }
                repo.SaveChanges();
                scope.Complete();
            }

        }

        public static void SaveFGRN(DeliveryChallan dc)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new FGRNRepository();
                if (dc.Id == 0)
                {
                    dc.CreatedDate = DateTime.Now;
                    dc.VoucherNumber = repo.GetNextVoucherNumber(dc.TransactionType);
                    repo.Save(dc, repo);
                }
                else
                {
                    repo.Update(dc, repo);
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

                new TransactionRepository().HardDelete(voucherno, transactiontype);
                new DeliveryChallanRepository().DeleteByVoucherNumber(voucherno);
                scope.Complete();
            }

        }
        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var dc = new DeliveryChallanRepository().GetById(id);
                new TransactionRepository().HardDelete(dc.VoucherNumber, dc.TransactionType);
                new DeliveryChallanRepository().Delete(dc);
                scope.Complete();
            }

        }


        public static void AddTransaction(DeliveryChallan s, BaseRepository saleRepo)
        {
            var dt = DateTime.Now;
            var repo = new TransactionRepository(saleRepo);
            repo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>();
            if (SettingManager.AllowCGS)
            {
                var balances = new TransactionRepository().GetStockAvgRates(s.DCItems.Select(p => p.ItemId).Distinct().ToList(), s.Date);
                var items = s.DCItems.CloneWithJson();
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
                    AccountId = SettingManager.StockValueAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.InventoryConsumpation,
                    Credit = item.Amount,
                    Comments = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")"


                }).ToList());

                trans.AddRange(items.Select(item => new Transaction
                {
                    AccountId = SettingManager.StockConsumptionAccountId,
                    Quantity = Numerics.GetInt(item.Quantity),
                    Price = Numerics.GetDecimal(item.Rate),
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.InventoryConsumpation,
                    Debit = item.Amount,
                    Comments = item.ItemName + " (" + Math.Round(item.Quantity, 2) + "X" + Math.Round(item.Rate, 2) + ")"

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
            repo.Add(trans);
        }


        public static string ValidateSave(DeliveryChallan input)
        {
            return ValidateSave(input, false);
        }
        public static string ValidateSave(DeliveryChallan input, bool allowDupliateItem)
        {
            return ValidateSave(input, allowDupliateItem, false);
        }
        public static string ValidateSave(DeliveryChallan input, bool allowDupliateItem, bool allowDupliateBookNo)
        {
            return ValidateSave(input, allowDupliateItem, allowDupliateBookNo, true);
        }

        public static string ValidateSave(DeliveryChallan input, bool allowDupliateItem, bool allowDupliateBookNo, bool isAccountRequired)
        {
            var err = ",";
            try
            {
                var repo = new DeliveryChallanRepository();
                var orderbookingrepo = new OrderBookingRepository(repo);
                var salerepo = new SaleRepository(repo);


                var type = input.TransactionType == VoucherType.GoodIssue ? VoucherType.SaleOrder : VoucherType.PurchaseOrder;
                var record = repo.GetByVoucherNumber(input.VoucherNumber, type, input.Id);

                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new voucher.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update voucher.,";
                        }
                    }
                }

                if (isAccountRequired)
                {
                    if (input.AccountId == 0)
                    {
                        err += "Account is not valid to process the request.,";
                    }
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Date should be within current fiscal year.,";
                }
                foreach (var item in input.DCItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }

                var productionStatueses = new List<byte> { (byte)TransactionStatus.FGRN, (byte)TransactionStatus.Pending, (byte)TransactionStatus.PartialyDelivered };
                if (!allowDupliateBookNo)
                {
                    record = repo.GetByBookNumber(input.InvoiceNumber, input.TransactionType, input.Id);

                    if (record != null)
                    {
                        err += "Book no already exist.,";
                    }
                }
                var settingAllowRate = true;
                if (input.TransactionType == VoucherType.GoodIssue)
                {
                    settingAllowRate = SettingManager.IsGINAllowRate;
                }
                else if (input.TransactionType == VoucherType.GoodReceive)
                {
                    settingAllowRate = SettingManager.IsGRNAllowRate;
                }

                if (settingAllowRate)
                {
                    foreach (var item in input.DCItems.Where(p => p.Rate <= 0 || p.Amount <= 0))
                    {
                        err += item.ItemCode + "-" + item.ItemName + " must have Rate and Amount greater than zero(0).,";
                    }
                }
                if (!allowDupliateItem)
                {
                    var Itemcountlist = input.DCItems.GroupBy(p => p.ItemId).Select(p => new
                    {
                        ItemId = p.Key,
                        ItemCode = p.FirstOrDefault().ItemCode,
                        ItemName = p.FirstOrDefault().ItemName,
                        Count = p.Count()
                    }).Where(p => p.Count > 1).ToList();

                    foreach (var item in Itemcountlist)
                    {
                        err += item.ItemCode + "-" + item.ItemName + " must be added once in item list.(Current Count:" + item.Count + "),";
                    }
                }

                if (input.OrderNo > 0)
                {
                    var order = orderbookingrepo.GetByVoucherNumber(input.OrderNo, type);
                    if (order == null)
                    {
                        err += "Invalid order no.,";
                    }
                    else
                    {
                        if (input.TransactionType == VoucherType.GoodIssue && (order.OrderType == (byte)OrderType.Production && !productionStatueses.Contains(order.Status)))
                        {
                            err += "order is not valid to be used in goods notes.,";
                        }
                        if (input.AccountId != order.AccountId)
                        {
                            err += "order and delivery challan party must be same.,";
                        }
                        if (repo.IsOrderLinkWithOtherParty(order.Id, input.AccountId.Value))
                        {
                            err += "order can only be delivered to one party.,";
                        }

                        //foreach (var item in input.DCItems)
                        //{
                        //    if (!order.OrderItems.Any(p => p.ItemId == item.ItemId))
                        //        err += item.ItemCode + "-" + item.ItemName + " is not included in current order.,";
                        //}

                        var goodsNotes = repo.GetByOrderNumber(input.TransactionType, input.OrderNo)
                         .Where(p => p.VoucherNumber != input.VoucherNumber).SelectMany(p => p.DCItems).GroupBy(p => p.ItemId).Select(p => new
                         {
                             ItemId = p.Key,
                             Quantity = p.Sum(q => q.Quantity)
                         }).ToList();
                        foreach (var item in input.DCItems)
                        {
                            var dbQty = item.Quantity;
                            var currentQty = item.Quantity;
                            var GNqty = 0.0M;
                            var totalQty = 0.0M;
                            if (goodsNotes.Any(p => p.ItemId == item.ItemId))
                                GNqty = goodsNotes.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                            totalQty = GNqty + currentQty;
                            if (order.OrderItems.Any(p => p.ItemId == item.ItemId))
                            {
                                var orderQty = order.OrderItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                                if (totalQty > orderQty)
                                {
                                    err += item.ItemCode + "-" + item.ItemName + " maximum quantity reached.(total:" + orderQty + " delivered:" + GNqty + ").remaining quantity is " + (orderQty - GNqty) + ",";
                                }
                            }
                            else
                            {
                                err += item.ItemCode + "-" + item.ItemName + " is not included in current order.,";
                            }
                        }
                    }
                }
                if (input.Id > 0)
                {
                    var dbGoodsNotes = repo.GetById(input.Id);
                    if (dbGoodsNotes.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change DC no.please use previous DC no.(" + dbGoodsNotes.VoucherNumber + "),";
                    }
                    if (dbGoodsNotes.OrderNo != input.OrderNo)
                    {
                        err += "can't change order no.please use previous order no.(" + dbGoodsNotes.OrderNo + "),";
                    }
                    if (input.OrderNo > 0)
                    {
                        var order = orderbookingrepo.GetByVoucherNumber(input.OrderNo, type);
                        if (order != null && order.Status == (byte)TransactionStatus.Invoiced)
                        {
                            err += "Order is already processed and goods note can't be updated.,";
                        }
                    }
                    var vouchertypes = new List<VoucherType>();
                    if (input.TransactionType == VoucherType.GoodIssue)
                        vouchertypes = new List<VoucherType> { VoucherType.Sale, VoucherType.GstSale };
                    else
                        vouchertypes = new List<VoucherType> { VoucherType.Purchase, VoucherType.GstPurchase };
                    var saleList = salerepo.GetByDCNo(vouchertypes, input.VoucherNumber)
                  .SelectMany(p => p.SaleItems).GroupBy(p => p.ItemId).Select(p => new
                  {
                      ItemId = p.Key,
                      Quantity = p.Sum(q => q.Quantity)
                  }).ToList();
                    var itemIds = input.DCItems.Select(p => p.ItemId).ToList();
                    var deletedrecord = dbGoodsNotes.DCItems.Where(p => !itemIds.Contains(p.ItemId)).ToList();
                    foreach (var item in dbGoodsNotes.DCItems)
                    {
                        var Dbqty = item.Quantity;//6
                        var Newqty = 0.0M;//2//2-6=-4
                        var saleqty = 0.0M;//3
                        if (saleList.Any(p => p.ItemId == item.ItemId))
                            saleqty = saleList.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;

                        if (input.DCItems.Any(p => p.ItemId == item.ItemId))
                        {
                            Newqty = input.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                            if (Newqty < saleqty)
                            {
                                err += item.ItemCode + "-" + item.ItemName + " has already invoice of " + saleqty + " quantity.,";
                            }
                        }
                        else if (saleqty > 0)
                        {
                            err += item.ItemCode + "-" + item.ItemName + " can't be deleted.it has already invoice of " + saleqty + " quantity.,";

                        }
                    }
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;

        }

        public static string ValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var repo = new DeliveryChallanRepository();
                var invoicedscrepo = new InvoiceDcsRepository(repo);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete the voucher.,";
                    }
                }
                else
                {
                    var GN = new DeliveryChallanRepository().GetById(id);
                    if (GN != null)
                    {
                        var vouchertypes = new List<VoucherType>();
                        if (GN.TransactionType == VoucherType.GoodIssue)
                            vouchertypes = new List<VoucherType> { VoucherType.Sale, VoucherType.GstSale };
                        else
                            vouchertypes = new List<VoucherType> { VoucherType.Purchase, VoucherType.GstPurchase };
                        if (invoicedscrepo.CheckIfSaleExistByDCId(GN.Id))
                            err += "goods note is used in invoice note can't be deleted.";
                    }
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
    }
}
