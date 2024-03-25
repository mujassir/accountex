using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class RentOpeningBalanceManager
    {


        public static void Save(RentOpeningBalance input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentOpeningBalanceRepository();
                repo.Save(input, true, false);
                repo.SaveChanges();
                AddTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void AddTransaction(RentOpeningBalance rob, BaseRepository repo)
        {


            var transRepo = new TransactionRepository(repo);
            transRepo.HardDeleteByReferenceIdTransactionType(rob.Id, VoucherType.ROB);
            var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.ROB);
            var item = new RentOpeningBalanceRepository().GetByIdFromView(rob.Id);
            var dt = DateTime.Now;
            var trans = new List<Transaction>();

            if (item != null)
            {
                var comment = "BBF for the tenant " + item.TenantAccountName + " against shop no." + item.ShopNo;
                //1. Monthly Rent Debit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = item.TenantAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.MonthlyRent,
                    VoucherNumber = voucherNo,
                    Debit = item.Rent,
                    CreatedDate = dt,
                    Comments = "Monthly Rent " + comment,
                    ReferenceId = item.Id,

                });
                //2. Monthly Rent Credit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = SettingManager.MonthlyRentAcId,
                    AccountTitle = null,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.MonthlyRent,
                    VoucherNumber = voucherNo,
                    Credit = item.Rent,
                    CreatedDate = dt,
                    Comments = "Monthly Rent " + comment,
                    ReferenceId = item.Id,

                });

                //3. Utliliy charges Debit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = item.TenantAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.UCPercent,
                    VoucherNumber = voucherNo,
                    Debit = item.UC,
                    CreatedDate = dt,
                    Comments = "Utliliy charges " + comment,
                    ReferenceId = item.Id,
                });
                //4. Monthly Rent Credit to Utliliy charge Acount
                trans.Add(new Transaction()
                {
                    AccountId = SettingManager.UtliliychargeAcId,
                    AccountTitle = null,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.UCPercent,
                    VoucherNumber = voucherNo,
                    Credit = item.UC,
                    CreatedDate = dt,
                    Comments = "Utliliy charges " + comment,
                    ReferenceId = item.Id,

                });
                //2. Elecricity charges Debit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = item.TenantAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.ElectricityCharges,
                    VoucherNumber = voucherNo,
                    Debit = item.Electricity,
                    CreatedDate = dt,
                    Comments = "Elecricity charges  " + comment,
                    ReferenceId = item.Id,

                });
                //2. Elecricity Credit to Electricity charge Account
                trans.Add(new Transaction()
                {
                    AccountId = SettingManager.ElectricitychargeAcId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.ElectricityCharges,
                    VoucherNumber = voucherNo,
                    Credit = item.Electricity,
                    CreatedDate = dt,
                    Comments = "Elecricity charges  " + comment,
                    ReferenceId = item.Id,

                });
                //4. Surcharge Debit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = item.TenantAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.Surcharge,
                    VoucherNumber = voucherNo,
                    Debit = item.SurCharge,
                    CreatedDate = dt,
                    Comments = "Surcharge  " + comment,
                    ReferenceId = item.Id,

                });
                //2. Electricity charges Credit to Electricity charge Account
                trans.Add(new Transaction()
                {
                    AccountId = SettingManager.SurchargeAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.ROB,
                    EntryType = (byte)EntryType.Surcharge,
                    VoucherNumber = voucherNo,
                    Credit = item.SurCharge,
                    CreatedDate = dt,
                    Comments = "Surcharge  " + comment,
                    ReferenceId = item.Id,

                });


                trans = trans.Where(p => p.Debit > 0 || p.Credit > 0).ToList();
                transRepo.Add(trans);
            }
        }

        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentOpeningBalanceRepository();
                var transactionRepo = new TransactionRepository(repo);
                repo.Delete(id);
                transactionRepo.HardDeleteByReferenceIdTransactionType(id, VoucherType.ROB);
                repo.SaveChanges();
                scope.Complete();
            }
        }

    }
}
