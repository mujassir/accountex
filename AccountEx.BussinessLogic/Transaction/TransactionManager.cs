using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using AccountEx.Repositories;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class TransactionManager
    {


        public static void Save(Sale sale)
        {
            Save(sale, true, true);
        }
        public static void Save(Sale sale, bool addTransaction)
        {
            Save(sale, addTransaction, false, true);
        }
        public static void Save(Sale sale, bool addTransaction, bool autoVoucherNo)
        {
            Save(sale, addTransaction, false, autoVoucherNo);
        }
        public static void Save(Sale sale, bool addTransaction, bool nextVoucherFromSale, bool autoVoucherNo)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new SaleRepository();
                var tranrRepo = new TransactionRepository(repo);
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    if (autoVoucherNo)
                        sale.VoucherNumber = nextVoucherFromSale ? repo.GetNextVoucherNumber(sale.TransactionType) : tranrRepo.GetNextVoucherNumber(sale.TransactionType);
                    //sale.InvoiceNumber = nextVoucherFromSale ? new SaleRepository().GetNextBookNumber(sale.TransactionType) : new TransactionRepository().GetNextInvoiceNumber(sale.TransactionType);

                    if (new List<VoucherType> { VoucherType.Sale, VoucherType.Purchase }.Contains(sale.TransactionType))
                    {
                        new GenericRepository<LogData>().Add(
                            new LogData
                            {
                                Data = $"Item " + (sale.TransactionType == VoucherType.Sale ? "Sold" : "Purchased"),
                                RecordId = sale.SaleItems.FirstOrDefault().ItemId
                            });
                    }
                    repo.Add(sale);
                }
                else
                {
                    repo.Update(sale);

                }
                if (addTransaction)
                {
                    switch (sale.TransactionType)
                    {
                        case VoucherType.Sale:
                        case VoucherType.Purchase:
                        case VoucherType.SaleReturn:
                        case VoucherType.PurchaseReturn:
                            AddTransaction(sale, sale.CashSale, repo);
                            break;
                        case VoucherType.GstSale:
                        case VoucherType.GstPurchase:
                        case VoucherType.GstSaleReturn:
                        case VoucherType.GstPurchaseReturn:
                            AddTransaction(sale, sale.CashSale, true, repo);
                            break;
                        default:
                            break;
                    }

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
            var saleRepo = new SaleRepository();
            var err = CGSManager.IsSaleExitAfterPurchase(voucherno, transactiontype, saleRepo);
            if (!string.IsNullOrWhiteSpace(err.Trim()))
                throw new OwnException(err);
            using (var scope = TransactionScopeBuilder.Create())
            {
                var tranRepo = new TransactionRepository(saleRepo);
                tranRepo.HardDelete(voucherno, transactiontype);
                saleRepo.DeleteByVoucherNumber(voucherno, transactiontype);
                saleRepo.SaveChanges();
                scope.Complete();
            }

        }
        public static void Delete(int voucherno, List<VoucherType> transactionTypes)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var tranRepo = new TransactionRepository();
                var saleRepo = new SaleRepository(tranRepo);
                tranRepo.HardDelete(voucherno, transactionTypes);
                saleRepo.DeleteByVoucherNumber(voucherno, transactionTypes);
                saleRepo.SaveChanges();
                scope.Complete();
            }

        }

        public static void AddTransaction(Sale s, bool isCashSale, SaleRepository saleRepo)
        {
            var dt = DateTime.Now;
            var repo = new TransactionRepository(saleRepo);
            repo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>
            {
                new Transaction
                {
                    ReferenceId=s.Id,
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
                      Comments=s.Comments,
                },
                new Transaction
                {
                    ReferenceId=s.Id,
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
                            : 0,
                      Comments=s.Comments,
                }
            };
            if (Numerics.GetInt(s.Discount) > 0)
            {
                trans.Add(new Transaction
                {
                    ReferenceId = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.DiscountAccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Comments = s.Comments,
                    Debit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.Discount)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.Discount)
                            : 0
                });


            }
            if (Numerics.GetInt(s.TotalFreight) > 0)
            {
                trans.Add(new Transaction
                {
                    ReferenceId = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.VehicleId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Credit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Comments = s.Comments,
                });
                trans.Add(new Transaction
                {
                    ReferenceId = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.FreightHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Discount,
                    Debit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Comments = s.Comments,
                });



            }
            //add cgs transaction if allowed
            trans.AddRange(CGSManager.GetTransaction(s, repo));
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
        public static void AddTransaction(Sale s, bool isCashSale, bool isGSTSale, SaleRepository saleRepo)
        {
            var dt = DateTime.Now;
            var repo = new TransactionRepository(saleRepo);
            repo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>
            {
                new Transaction
                {
                    ReferenceId=s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit =
                        s.TransactionType == VoucherType.GstSale ||
                        s.TransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new Transaction
                {
                    ReferenceId=s.Id,
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
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0
                }
            };
            if (Numerics.GetInt(s.GstAmountTotal) > 0)
            {
                trans.Add(new Transaction
                {
                    ReferenceId = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Credit = s.TransactionType == VoucherType.GstSale || s.TransactionType == VoucherType.GstPurchaseReturn
                  ? Numerics.GetInt(s.GstAmountTotal)
                  : 0,
                    Debit =
                        s.TransactionType == VoucherType.GstSaleReturn || s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.GstAmountTotal)
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
        public static string ValidateSave(Sale input)
        {
            return ValidateSave(input, false);
        }
        public static string ValidateSave(Sale input, bool allowDupliateItem)
        {
            return ValidateSave(input, allowDupliateItem, false);
        }
        public static string ValidateSave(Sale input, bool allowDupliateItem, bool allowDupliateBookNo)
        {
            return ValidateSave(input, allowDupliateItem, allowDupliateBookNo, true);
        }

        public static void SaveDairyTransaction(List<DairyTransaction> items)
        {
            var dt = DateTime.Now;
            var repo = new GenericRepository<DairyTransaction>();
            int voucherNumber = items.FirstOrDefault(x => x.VoucherNumber > 0)?.VoucherNumber
                ?? (repo.GetAll().OrderByDescending(p => p.VoucherNumber).FirstOrDefault()?.VoucherNumber ?? 0) + 1;

            var dbRecords = repo.GetAll(x => x.VoucherNumber == voucherNumber);
            foreach (var item in items)
            {
                var record = dbRecords.FirstOrDefault(x => x.Id == item.Id);
                if (record == null)
                {
                    item.VoucherNumber = voucherNumber;
                    item.CreatedAt = dt;
                    item.Date = item.Date == DateTime.MinValue ? dt : item.Date;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                    repo.Add(item);
                }
                else
                {
                    record.Date = item.Date == DateTime.MinValue ? dt : item.Date;
                    record.ItemId = item.ItemId;
                    record.ItemCode = item.ItemCode;
                    record.ItemName = item.ItemName;
                    record.Comment = item.Comment;
                    record.Qty = item.Qty;
                    record.Rate = item.Rate;
                    record.Amount = item.Amount;
                    record.TotalQty= item.TotalQty;
                    record.TotalAmount= item.TotalAmount;
                    record.EntryType = item.EntryType;

                    record.ModifiedAt = dt;
                    record.FiscalId = SiteContext.Current.Fiscal.Id;
                    repo.Update(record);
                }
            }

            var removedRecords = dbRecords.Where(x => !items.Where(e => e.Id > 0).Select(e => e.Id).Contains(x.Id));
            foreach (var item in removedRecords)
                repo.Delete(item.Id);

            repo.SaveChanges();
        }

        public static string ValidateSave(Sale input, bool allowDupliateItem, bool allowDupliateBookNo, bool isAccountRequired)
        {
            var err = ",";
            try
            {
                var saleRepo = new SaleRepository();
                var dcRepo = new DeliveryChallanRepository(saleRepo);
                var invoiceDCRepo = new InvoiceDcRepository(saleRepo);
                var GNType = input.TransactionType == VoucherType.Sale || input.TransactionType == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
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
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }

                var isExist = saleRepo.IsVoucherExits(input.VoucherNumber, input.TransactionType, input.Id);
                if (isExist)
                {
                    err += "Voucher no already exist.,";
                }
                if (!allowDupliateBookNo)
                {
                    isExist = saleRepo.IsBookNoExits(input.InvoiceNumber, input.TransactionType, input.Id);

                    if (isExist)
                    {
                        err += "Book no already exist.,";
                    }
                }
                if (input.InvoiceDcs.Count() > 0)
                {
                    var dcIds = input.InvoiceDcs.Select(p => p.DcId).ToList();
                    if (dcRepo.IsDcLinkWithOtherParty(dcIds, input.AccountId))
                    {
                        err += "Invoice and DC party must be same.,";
                    }
                }

                foreach (var item in input.SaleItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is not valid.,";
                }
                foreach (var item in input.SaleItems.Where(p => p.Rate <= 0 || p.Amount <= 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " must have Rate and Amount greater than zero(0).,";

                }

                if (!allowDupliateItem || SettingManager.CheckStockAvailability)
                {
                    var Itemcountlist = input.SaleItems.GroupBy(p => p.ItemId).Select(p => new
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
                }

                if (input.TransactionType == VoucherType.Sale && SettingManager.CheckStockAvailability)
                {
                    var balances = new TransactionRepository().GetStockOpeningBalance(input.SaleItems.Select(p => p.ItemId).Distinct().ToList());
                    var ditinctitems = input.SaleItems.GroupBy(p => p.ItemId).Select(p => new
                    {
                        ItemId = p.Key,
                        p.FirstOrDefault().ItemCode,
                        p.FirstOrDefault().ItemName,
                        Quantity = p.Sum(q => q.Quantity),
                        Count = p.Count()
                    });

                    foreach (var item in ditinctitems)
                    {
                        var stock = Numerics.GetInt(balances.FirstOrDefault(p => p.AccountId == item.ItemId).Balance);
                        //if (item.Count > 1)
                        //{
                        //    err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") must be added one time.,";
                        //}
                        var rb = 0;
                        if (input.Id == 0)
                        {

                            rb = stock - Numerics.GetInt(item.Quantity);
                            if (rb < 0)
                            {
                                err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available (" + stock + ").,";
                            }
                        }
                        else
                        {
                            var previousvoucher = new SaleRepository().GetById(input.Id);
                            var preitem = previousvoucher.SaleItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                            if (preitem == null)
                            {
                                rb = stock - Numerics.GetInt(item.Quantity);
                                if (rb < 0)
                                {
                                    err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available(" + stock + ").,";
                                }

                            }
                            else
                            {
                                var qty = Numerics.GetInt(item.Quantity) - Numerics.GetInt(preitem.Quantity);
                                rb = stock - qty;
                                if (rb < 0)
                                {
                                    err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available (" + stock + ").,";
                                }
                            }

                        }
                    }

                }



                if (input.InvoiceDcs.Count() > 0)
                {



                    foreach (var item in input.InvoiceDcs)
                    {
                        if (!dcRepo.CheckIfDcExist(item.DcId))
                        {
                            err += "Dc no. " + item.DcNumber + " is not valid.,";
                        }
                    }

                    foreach (var item in input.InvoiceDcs)
                    {
                        if (invoiceDCRepo.CheckIfSaleExistByDCId(item.DcId, item.SaleId))
                        {
                            err += "Dc no. " + item.DcNumber + " is already used.,";
                        }
                    }

                    //if (dc == null)
                    //{
                    //    err += "Invalid DC no.";
                    //}
                    //else
                    //{
                    //    var saleList = new SaleRepository().GetByDCNo(input.TransactionType, input.DCNo)
                    //                   .Where(p => p.VoucherNumber != input.VoucherNumber).SelectMany(p => p.SaleItems).GroupBy(p => p.ItemId).Select(p => new
                    //                   {
                    //                       ItemId = p.Key,
                    //                       Quantity = p.Sum(q => q.Quantity)
                    //                   }).ToList();
                    //    foreach (var item in input.SaleItems)
                    //    {
                    //        var dbQty = item.Quantity;
                    //        var currentQty = item.Quantity;
                    //        var saleQty = 0.0M;
                    //        var totalQty = 0.0M;
                    //        if (saleList.Any(p => p.ItemId == item.ItemId))
                    //            saleQty = saleList.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                    //        totalQty = saleQty + currentQty;
                    //        if (dc.DCItems.Any(p => p.ItemId == item.ItemId))
                    //        {
                    //            var DCQty = dc.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                    //            if (totalQty > DCQty)
                    //            {
                    //                err += item.ItemCode + "-" + item.ItemName + " maximum quantity reached.(total:" + DCQty + " delivered:" + saleQty + ").remaining quantity is " + (DCQty - saleQty) + ",";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            err += item.ItemCode + "-" + item.ItemName + " is not included in current Goods notes.,";

                    //        }



                    //    }
                    //}
                }
                err += CGSManager.IsSaleExitAfterPurchase(input, saleRepo);
                if (input.Id > 0)
                {
                    var dbSale = saleRepo.GetById(input.Id, true);
                    if (dbSale.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no.please use previous voucher no.(" + dbSale.VoucherNumber + "),";
                    }

                    //if (dbSale.DCNo != input.DCNo)
                    //{
                    //    err += "can't change dc no.please use previous dc no.(" + dbSale.DCNo + "),";
                    //}

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
