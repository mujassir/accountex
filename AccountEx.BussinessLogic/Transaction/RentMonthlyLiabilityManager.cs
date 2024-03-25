using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class RentMonthlyLiabilityManager
    {
        public static void Save(RentDetail input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentMonthlyLiabilityRepository();
                if (input.Id == 0)
                {
                    repo.Add(input, true, true);
                }
                else
                {
                    repo.UpdateRentLiability(input);
                }
                AddTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void AddTransaction(RentDetail rentDetail, BaseRepository repo)
        {


            var transRepo = new TransactionRepository(repo);
            // transRepo.HardDelete(rentDetail.VoucherNumber, VoucherType.RentMonthlyLiability);
            var items = rentDetail.RentDetailItems;
            var dt = DateTime.Now;
            var trans = new List<Transaction>();

            foreach (var item in items)
            {
                var comment = "for the tenant " + item.TenantAccountName + " against shop no." + item.ShopNo;

                //1. Monthly Rent Debit to TenantAccount
                trans.Add(new Transaction()
                {
                    AccountId = item.TenantAccountId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.MonthlyRent,
                    VoucherNumber = item.VoucherNumber,
                    Debit = item.MonthlyRent,
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
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.MonthlyRent,
                    VoucherNumber = item.VoucherNumber,
                    Credit = item.MonthlyRent,
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
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.UCPercent,
                    VoucherNumber = item.VoucherNumber,
                    Debit = item.UCPercent,
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
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.UCPercent,
                    VoucherNumber = item.VoucherNumber,
                    Credit = item.UCPercent,
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
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.ElectricityCharges,
                    VoucherNumber = item.VoucherNumber,
                    Debit = item.ElectricityCharges,
                    CreatedDate = dt,
                    Comments = "Elecricity charges  " + comment,
                    ReferenceId = item.Id,

                });
                //2. Electricity charges Credit to Electricity charge Account
                trans.Add(new Transaction()
                {
                    AccountId = SettingManager.ElectricitychargeAcId,
                    AccountTitle = "",
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.RentMonthlyLiability,
                    EntryType = (byte)EntryType.ElectricityCharges,
                    VoucherNumber = item.VoucherNumber,
                    Credit = item.ElectricityCharges,
                    CreatedDate = dt,
                    Comments = "Elecricity charges  " + comment,
                    ReferenceId = item.Id,

                });

            }
            trans = trans.Where(p => p.Debit > 0 || p.Credit > 0).ToList();
            transRepo.Add(trans);
        }

        public static void Delete(int id)
        {
            var challanRepo = new ChallanRepository();
            var transRepo = new TransactionRepository(challanRepo);
            var rentMonthlyLiability = challanRepo.GetById(id);
            foreach (var item in rentMonthlyLiability.ChallanItems)
            {
                transRepo.HardDelete(item.VoucherNumber, VoucherType.RentMonthlyLiability);
            }
            challanRepo.Delete(id);
        }
        public static void DeleteElectricity(int id)
        {
            var challanRepo = new ChallanRepository();
            var transRepo = new TransactionRepository(challanRepo);
            var rentMonthlyLiability = challanRepo.GetById(id);

            transRepo.HardDeleteByReferenceIdTransactionType(id, VoucherType.ElectictyChallan);

            challanRepo.Delete(id);
        }
        public static void DeleteRent(int id)
        {
            var challanRepo = new ChallanRepository();
            var transRepo = new TransactionRepository(challanRepo);
            var rentMonthlyLiability = challanRepo.GetById(id);
            transRepo.HardDeleteByReferenceIdTransactionType(id, VoucherType.RC);
            challanRepo.Delete(id);
        }
        public static void DeleteForSingleLiability(int id)
        {
            var rentDetailItemRepo = new RentDetailItemRepository();
            var transRepo = new TransactionRepository(rentDetailItemRepo);
            var rentMonthlyLiability = rentDetailItemRepo.GetById(id);
            transRepo.HardDeleteByReferenceIdTransactionType(rentMonthlyLiability.Id, VoucherType.RentMonthlyLiability);
            rentDetailItemRepo.Delete(id);
        }

    }
}
