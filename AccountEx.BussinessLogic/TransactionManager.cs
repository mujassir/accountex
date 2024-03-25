using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class TransactionManager
    {


        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new SaleRepository();
                if (sale.Id == 0)
                {
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = new TransactionRepository().GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale);
                    repo.SaveLog(sale, ActionType.Added);

                }
                else
                {
                    repo.Update(sale);
                    repo.SaveLog(sale, ActionType.Updated);

                }
                switch (sale.TransactionType)
                {
                    case (byte)VoucherType.GstPurchase:
                    case (byte)VoucherType.GstPurchaseReturn:
                    case (byte)VoucherType.GstSale:
                    case (byte)VoucherType.GstSaleReturn:
                        AddTransaction(sale, sale.CashSale,true);
                        break;
                    default:
                        AddTransaction(sale, sale.CashSale);
                        break;
                }


                scope.Complete();
            }

        }

        public static Sale GetVocuherDetail(int voucherno, byte transactiontype, string key)
        {
            var d = new Sale();
            bool next, previous;
            d = new SaleRepository().GetByVoucherNumber(voucherno, transactiontype, key, out next, out previous);
            return d;
        }

        public static void Delete(int voucherno, byte transactiontype)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                new TransactionRepository().HardDelete(voucherno, transactiontype);
                new SaleRepository().DeleteByVoucherNumber(voucherno, transactiontype);
                scope.Complete();
            }

        }
        public static void AddTransaction(Sale s, bool isCashSale)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(s.VoucherNumber, s.TransactionType);
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
                        s.TransactionType == (byte) VoucherType.Sale ||
                        s.TransactionType == (byte) VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == (byte) VoucherType.SaleReturn ||
                        s.TransactionType == (byte) VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == (byte) VoucherType.Sale
                            ? SettingManager.SaleAccountHeadId
                            : s.TransactionType == (byte) VoucherType.Purchase
                                ? SettingManager.PurchaseAccountHeadId
                                : s.TransactionType == (byte) VoucherType.SaleReturn
                                    ? SettingManager.SaleReturnAccountHeadId
                                    : SettingManager.PurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit =
                        s.TransactionType == (byte) VoucherType.Sale ||
                        s.TransactionType == (byte) VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0,
                    Debit =
                        s.TransactionType == (byte) VoucherType.SaleReturn ||
                        s.TransactionType == (byte) VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
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

            //    Credit = s.TransactionType == (byte)VoucherType.Sale || s.TransactionType == (byte)VoucherType.PurchaseReturn
            //            ? Numerics.GetInt(item.Amount)
            //            : 0,
            //    Debit =
            //        s.TransactionType == (byte)VoucherType.SaleReturn || s.TransactionType == (byte)VoucherType.Purchase
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
                    Debit = s.TransactionType == (byte)VoucherType.Sale || s.TransactionType == (byte)VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.Discount)
                            : 0,
                    Credit =
                        s.TransactionType == (byte)VoucherType.SaleReturn || s.TransactionType == (byte)VoucherType.Purchase
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
                    Credit = s.TransactionType == (byte)VoucherType.Sale || s.TransactionType == (byte)VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Debit =
                        s.TransactionType == (byte)VoucherType.SaleReturn || s.TransactionType == (byte)VoucherType.Purchase
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
                    Debit = s.TransactionType == (byte)VoucherType.Sale || s.TransactionType == (byte)VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.TotalFreight)
                            : 0,
                    Credit =
                        s.TransactionType == (byte)VoucherType.SaleReturn || s.TransactionType == (byte)VoucherType.Purchase
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
            new TransactionRepository().Add(trans);
        }
        public static void AddTransaction(Sale s, bool isCashSale, bool isGSTSale)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(s.VoucherNumber, s.TransactionType);
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
                        s.TransactionType == (byte) VoucherType.GstSale ||
                        s.TransactionType == (byte) VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == (byte) VoucherType.GstSaleReturn ||
                        s.TransactionType == (byte) VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == (byte) VoucherType.GstSale
                            ? SettingManager.GstSaleAccountHeadId
                            : s.TransactionType == (byte) VoucherType.GstPurchase
                                ? SettingManager.GstPurchaseAccountHeadId
                                : s.TransactionType == (byte) VoucherType.GstSaleReturn
                                    ? SettingManager.GstSaleReturnAccountHeadId
                                    : SettingManager.GstPurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit =
                        s.TransactionType == (byte) VoucherType.GstSale ||
                        s.TransactionType == (byte) VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0,
                    Debit =
                        s.TransactionType == (byte) VoucherType.GstSaleReturn ||
                        s.TransactionType == (byte) VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0
                }
            };
            if (Numerics.GetInt(s.GstAmountTotal) > 0)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Credit = s.TransactionType == (byte)VoucherType.GstSale || s.TransactionType == (byte)VoucherType.GstPurchaseReturn
                  ? Numerics.GetInt(s.GstAmountTotal)
                  : 0,
                    Debit =
                        s.TransactionType == (byte)VoucherType.GstSaleReturn || s.TransactionType == (byte)VoucherType.GstPurchase
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
            new TransactionRepository().Add(trans);
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

        public static string ValidateSave(Sale input, bool allowDupliateItem, bool allowDupliateBookNo, bool isAccountRequired)
        {
            var err = ",";
            try
            {
                var GNType = input.TransactionType == (byte)VoucherType.Sale || input.TransactionType == (byte)VoucherType.GstSale ? (byte)VoucherType.GoodIssue : (byte)VoucherType.GoodReceive;
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
                var record = new SaleRepository().GetByVoucherNumber(input.VoucherNumber, input.TransactionType, input.Id);
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                if (!allowDupliateBookNo)
                {
                    record = new SaleRepository().GetByBookNumber(input.InvoiceNumber, input.TransactionType, input.Id);

                    if (record != null)
                    {
                        err += "Book no already exist.,";
                    }
                }

                foreach (var item in input.SaleItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is not valid.,";
                }
                if (!allowDupliateItem)
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

                if (input.InvoiceDcs.Count() > 0)
                {



                    foreach (var item in input.InvoiceDcs)
                    {
                        if (!new DeliveryChallanRepository().CheckIfDcExist(item.DcId))
                        {
                            err += "Dc no. " + item.DcNumber + " is not valid.,";
                        }
                    }

                    foreach (var item in input.InvoiceDcs)
                    {
                        if (new InvoiceDcRepository().CheckIfSaleExistByDCId(item.DcId, item.SaleId))
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


                if (input.Id > 0)
                {
                    var dbSale = new SaleRepository().GetById(input.Id);
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
