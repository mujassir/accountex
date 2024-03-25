using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class BankReceiptManager
    {
        public static void Save(BankReceipt input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                //var repo = new BankReceiptRepository();
                //repo.Save(input);
                var repo = new BankReceiptRepository();
                if (input.Id == 0)
                {
                    repo.Add(input, true, false);
                    foreach (var item in input.BankReceiptItems)
                    {
                        var challanItem = new GenericRepository<ChallanItem>().AsQueryable().FirstOrDefault(p => p.Id == item.ChallanItemId);
                        challanItem.IsReceived = true;
                        new ChallanItemRepository().Update(challanItem);
                    }
                }
                else
                {
                    repo.Update(input);
                }
                AddTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void AddTransaction(BankReceipt bankReceipt, BaseRepository baseRepo)
        {
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(bankReceipt.VoucherNumber, VoucherType.FortressBankReceipt);
            var items = bankReceipt.BankReceiptItems;
            var dt = DateTime.Now;
            var trans = new List<Transaction>();
            foreach (var item in items)
            {
                if (item.MonthlyRent > 0 && item.UCPercent > 0 && item.ElectricityCharges > 0)
                {
                    //1. Monthly Rent Debit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        AccountTitle = "Monthly Rent",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.MonthlyRent,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Debit = item.MonthlyRent,
                        CreatedDate = dt,
                        Comments = "Monthly Rent Debit to TenantAccount",

                    });
                    //2. Elecricity charges Debit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        AccountTitle = "Elecricity charges",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.ElectricityCharges,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Debit = item.ElectricityCharges,
                        CreatedDate = dt,
                        Comments = "Elecricity charges Debit to TenantAccount.",

                    });
                    //3. Utliliy charges Debit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        AccountTitle = "Utliliy charges",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.UCPercent,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Debit = item.UCPercent,
                        CreatedDate = dt,
                        Comments = "Utliliy charges Debit to TenantAccount.",

                    });
                    //4. Monthly Rent Credit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankHeadId,
                        AccountTitle = "Monthly Rent",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.MonthlyRent,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Credit = item.MonthlyRent,
                        CreatedDate = dt,
                        Comments = "Monthly Rent Credit to Bank Account",

                    });
                    //5. Elecricity charges Credit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankHeadId,
                        AccountTitle = "Elecricity charges",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.ElectricityCharges,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Credit = item.ElectricityCharges,
                        CreatedDate = dt,
                        Comments = "Elecricity charges Credit to Bank Account",

                    });
                    //6. Utliliy charges Credit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankHeadId,
                        AccountTitle = "Utliliy charges",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.UCPercent,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Credit = item.UCPercent,
                        CreatedDate = dt,
                        Comments = "Utliliy charges Credit to Bank Account",

                    });
                }
                else
                {
                    //1. Amount Debit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        AccountTitle = "",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Debit = item.Amount,
                        CreatedDate = dt,
                        Comments = "Monthly Rent Debit to TenantAccount",

                    });
                    //2. Amount Credit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankHeadId,
                        AccountTitle = "",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        VoucherNumber = bankReceipt.VoucherNumber,
                        Credit = item.Amount,
                        CreatedDate = dt,
                        Comments = "Rent Credit to Monthly Rent Account",

                    });
                }
            }
            transRepo.Add(trans);
        }

        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ChallanRepository();
                var challanItemRepo = new ChallanRepository(repo);
                var transactionRepo = new TransactionRepository(repo);
                var challan = repo.GetById(id);
                if (challan != null)
                {
                    challan.IsReceived = false;
                    challan.ReceiveDate = null;
                    challan.RcvNo = 0;
                    transactionRepo.HardDeleteByReferenceIdTransactionType(challan.Id, VoucherType.FortressBankReceipt);
                }
                repo.SaveChanges();
                scope.Complete();



            }

        }
        public static void DeleteByVoucherNo(int voucherNo)
        {
            DeleteByVoucherNo(voucherNo, null);
        }
        public static void DeleteByVoucherNo(int voucherNo, List<Challan> challans)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new BankReceiptRepository();
                var challanRepo = new ChallanRepository(repo);
                var transactionRepo = new TransactionRepository(repo);
                if (challans == null)
                    challans = challanRepo.GetByRcvNo(voucherNo);
                foreach (var challan in challans)
                {
                    challan.IsReceived = false;
                    challan.ReceiveDate = null;
                    challan.RcvNo = 0;
                }
                transactionRepo.HardDelete(voucherNo, VoucherType.FortressBankReceipt);
                repo.SaveChanges();
                scope.Complete();
            }

        }

    }
}
