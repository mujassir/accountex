using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class JobCardManager
    {
        public static void Save(Sale sale)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new TransRepository();
               var transactionRepo= new TransactionRepository(repo);
                var saleItemRepo=new SaleItemRepository(repo);
                if (sale.Id == 0)
                {
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = SettingManager.Comission;
                        x.ComissionAmount = x.Amount * (SettingManager.Comission / 100);
                    });
                    sale.CreatedDate = DateTime.Now;
                    sale.VoucherNumber = transactionRepo.GetNextVoucherNumber(sale.TransactionType);
                    repo.Add(sale,true,false);
                }
                else
                {
                    var prevcomission = saleItemRepo.GetBySaleId(sale.Id).ComissionPercent;
                    sale.SaleItems.ToList().ForEach(x =>
                    {
                        x.ComissionPercent = prevcomission;
                        x.ComissionAmount = x.Amount * (prevcomission / 100);
                    });
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
        public static void Delete(int voucherno, VoucherType transactiontype, int id)
        {
            var saleRepo = new SaleRepository();
            var transRepo = new TransactionRepository(saleRepo);
            var saleSIRepo= new SaleServicesItemRepository(saleRepo);
            var serviceEXRepo= new ServiceExpensesRepository(saleRepo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                saleSIRepo.DeleteBySaleId(id);
                serviceEXRepo.DeleteBySaleId(id);
                transRepo.HardDelete(voucherno, transactiontype);
                saleRepo.DeleteByVoucherNumber(voucherno, transactiontype);

                scope.Complete();
            }

        }
        public static void AddTransaction(Sale s, bool isCashSale, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(s.VoucherNumber, s.TransactionType);
            var trans = new List<Transaction>
            {

                //Debit NetAmount to Customer Account
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit =Numerics.GetInt(s.NetTotal)
                },

                 //Credit GrossAmount to Customer Account
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =  SettingManager.ServicesAccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Credit = Numerics.GetInt(s.GrossTotal)
                },

                //Credit GstAmount to Customer Account
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.GstServicesHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.Gst,
                    Credit = Numerics.GetInt(s.GstAmountTotal)
                },

                 //Credit ServicesExpencesTotal to Stock Consumption Account
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.StockConsumptionAccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.HeadAccount,
                    Credit = Numerics.GetInt(s.ServicesExpencesTotal)
                },

                 //Debit ServicesExpencesTotal to Service Expenses Account
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.ServiceExpensesHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte)EntryType.ServiceExpense,
                    Debit = Numerics.GetInt(s.ServicesExpencesTotal)
                }
            };
            foreach (var item in trans)
            {
                item.VoucherNumber = s.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = s.Date == DateTime.MinValue ? dt : s.Date;
                item.Comments = s.Comments;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans);
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
                var saleRepo = new SaleRepository();
                var dcRepo=new DeliveryChallanRepository(saleRepo);
                var GNType = input.TransactionType  == VoucherType.Sale || input.TransactionType  == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
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
                    var servicescountlist = input.SaleServicesItems.GroupBy(p => p.ServiceItemItemId).Select(p => new
                        {
                            ServiceItemId = p.Key,
                            ServiceItemCode = p.FirstOrDefault().ServiceItemCode,
                            ServiceItemName = p.FirstOrDefault().ServiceItemName,
                            Count = p.Count()
                        }).Where(p => p.Count > 1).ToList();
                    foreach (var item in servicescountlist)
                    {
                        err += item.ServiceItemCode + "-" + item.ServiceItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                    }
                }

                if (input.DCNo > 0)
                {

                    var dc = dcRepo.GetByVoucherNumber(input.DCNo, GNType);

                    if (dc == null)
                    {
                        err += "Invalid DC no.";
                    }
                    else
                    {
                        //var saleList = new SaleRepository().GetByDCNo(input.TransactionType, input.DCNo)
                        //               .Where(p => p.VoucherNumber != input.VoucherNumber).SelectMany(p => p.SaleItems).GroupBy(p => p.ItemId).Select(p => new
                        //               {
                        //                   ItemId = p.Key,
                        //                   Quantity = p.Sum(q => q.Quantity)
                        //               }).ToList();
                        //foreach (var item in input.SaleItems)
                        //{
                        //    var dbQty = item.Quantity;
                        //    var currentQty = item.Quantity;
                        //    var saleQty = 0;
                        //    var totalQty = 0;
                        //    if (saleList.Any(p => p.ItemId == item.ItemId))
                        //        saleQty = saleList.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                        //    totalQty = saleQty + currentQty;
                        //    if (dc.DCItems.Any(p => p.ItemId == item.ItemId))
                        //    {
                        //        var DCQty = dc.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId).Quantity;
                        //        if (totalQty > DCQty)
                        //        {
                        //            err += item.ItemCode + "-" + item.ItemName + " maximum quantity reached.(total:" + DCQty + " delivered:" + saleQty + ").remaining quantity is " + (DCQty - saleQty) + ",";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        err += item.ItemCode + "-" + item.ItemName + " is not included in current Goods notes.,";

                        //    }
                        //}
                    }
                }


                if (input.Id > 0)
                {
                    var dbSale = saleRepo.GetById(input.Id);
                    if (dbSale.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no.please use previous voucher no.(" + dbSale.VoucherNumber + "),";
                    }

                    if (dbSale.DCNo != input.DCNo)
                    {
                        err += "can't change dc no.please use previous dc no.(" + dbSale.DCNo + "),";
                    }

                }
                if (input.EmployeeId == 0)
                {
                    err += "Service Rep is required.";
                }
                if (input.MachineId == 0)
                {
                    err += "Equipment is required.";
                }
                //var data = new SaleItemRepository().CheckIfTaxNoExist(input.SaleItems.Select(p => p.SaleTaxNo).ToList(), input.SaleItems.Select(q => q.Id).ToList());

                //if (data != null)
                //{
                //    err += "SaleTax no " + data.SaleTaxNo + " already exist.";
                //}
                //if (input.SaleItems.Select(p => p.SaleTaxNo).Count() != input.SaleItems.Select(p => p.SaleTaxNo).Distinct().Count())
                //{
                //    err += "Duplicate saletax no exist";
                //}
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
