using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Globalization;

namespace AccountEx.BussinessLogic
{
    public static class PayrollManager
    {
        private const string AccountOvertime = "Over Time";
        private const string AccountSalaryExpense = "Salary Expense";
        private const string AccountMedicalAllownce = "Medical Allownce";
        private const string AccountMedicalSecurity = "Medical Security";
        private const string AccountHouseRentAllownce = "House Rent Allownce";
        private const string AccountEtcAllownce = "ETC Allownce";
        private const string AccountConvyence = "Convyence";
        private const string AccountSocialSecurity = "Social Security";
        private const string AccountEobi = "EOBI";
        private const string AccountProvidentFund = "Provident Fund";
        private const string AccountInsurance = "Insurance";
        private const string AccountIncomeTax = "Income Tax";
        private static string commentsCompany = "Salary Approved by " + SiteContext.Current.User.Username;
        private const string AccountEmloyee = "Employee Account";

        public static void ProcessSalary(int month, int year)
        {
            var repo = new SalaryRepository();
            var salaryItems = repo.GetByMonthYear(true, month, year);
            foreach (var item in salaryItems)
            {
                item.IsProcessed = true;
                AddSalary(item);
            }
            repo.SaveChanges();

        }


        public static void AddSalary(Salary s)
        {
            s.IsProcessed = true;
            var trans = new List<Transaction>();
            var dt = DateTime.Now;
            s.Year = DateTime.Now.Year;

            //          Debit Entries

            new TransactionRepository().HardDelete(s.VoucherNumber, VoucherType.Salary);

            s.Year = s.PaymentDate.HasValue ? s.PaymentDate.Value.Year : dt.Year;
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.Month);

            new SalaryRepository().Save(s);

            if (s.BasicSalary != null)
                trans.Add(new Transaction
                {

                    AccountId = AccountManager.GetLeafAccountId(AccountSalaryExpense),
                    AccountTitle = AccountSalaryExpense,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.Item,
                    Debit = s.BasicSalary.Value,
                    //Comments = "Basic Salary"
                    Comments = "Basic Salary for the month of " + month + "-" + s.Year + " against " + s.Name,

                });
            //if (s.TotalOTCost.HasValue && s.TotalOTCost.Value > 0)
            //    trans.Add(new Transaction
            //    {
            //        AccountId = AccountManager.GetLeafAccountId(AccountHouseRentAllownce),
            //        AccountTitle = AccountHouseRentAllownce,
            //        TransactionType = VoucherType.Salary,
            //        EntryType = (byte)EntryType.HouseRent,
            //        Debit = s.TotalOTCost.Value
            //    });
            if (s.HouseRent.HasValue && s.HouseRent.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountHouseRentAllownce),
                    AccountTitle = AccountHouseRentAllownce,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.HouseRent,
                    Debit = s.HouseRent.Value,
                    //Comments = "House Rent"
                    Comments = "House Rent for the month of " + month + "-" + s.Year + " against " + s.Name,

                });
            if (s.Medical.HasValue && s.Medical.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountMedicalAllownce),
                    AccountTitle = AccountMedicalAllownce,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.MedicalAllownce,
                    Debit = s.Medical.Value,
                    //Comments = "Medical Allowance"
                    Comments = "Medical Allowance for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            //check this entry
            if (s.Conveyance.HasValue && s.Conveyance.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountConvyence),
                    AccountTitle = AccountConvyence,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.ConvyenceAllownce,
                    Debit = s.Conveyance.Value,
                    //Comments = "Conveyance Allowance"
                    Comments = "Conveyance Allowance for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            if (s.TotalOTCost.HasValue && s.TotalOTCost.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountOvertime),
                    AccountTitle = AccountOvertime,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.OverTime,
                    Debit = s.TotalOTCost.Value,
                    //Comments = "Overtime Cost"
                    Comments = "Overtime Cost for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            //          Credit Entries

            if (s.NetSalary.HasValue && s.NetSalary.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = s.AccountId,
                    AccountTitle = s.Name,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.MasterDetail,
                    Credit = s.NetSalary.Value,
                    //Comments = "Net Salary"
                    Comments = "Net Salary for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            //check this entry
            if (s.Installment.HasValue && s.Installment.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = s.AccountId,
                    AccountTitle = s.Name,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.Installement,
                    Credit = s.Installment.Value,
                    //Comments = "Installment"
                    Comments = "Installment for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            if (s.EOBI.HasValue && s.EOBI.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountEobi),
                    AccountTitle = AccountEobi,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.Eobi,
                    Credit = s.EOBI.Value,
                    //Comments = "EOBI"
                    Comments = "EOBI for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            if (s.ProvidentFund.HasValue && s.ProvidentFund.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountProvidentFund),
                    AccountTitle = AccountProvidentFund,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.ProvidentFund,
                    Credit = s.ProvidentFund.Value,
                    //Comments = "Provident Salary"
                    Comments = "Provident Salary for the month of " + month + "-" + s.Year + " against " + s.Name,

                });
            if (s.Insurance.HasValue && s.Insurance.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountInsurance),
                    AccountTitle = AccountInsurance,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.Insurance,
                    Credit = s.Insurance.Value,
                    //Comments = "Insurance"
                    Comments = "Insurance for the month of " + month + "-" + s.Year + " against " + s.Name,
                });
            if (s.IncomeTax.HasValue && s.IncomeTax.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountIncomeTax),
                    AccountTitle = AccountIncomeTax,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.IncomeTax,
                    Credit = s.IncomeTax.Value,
                    //Comments = "Income Tax"
                    Comments = "Income Tax for the month of " + month + "-" + s.Year + " against " + s.Name,

                });
            if (s.SocialSecurity.HasValue && s.SocialSecurity.Value > 0)
                trans.Add(new Transaction
                {
                    AccountId = AccountManager.GetLeafAccountId(AccountSocialSecurity),
                    AccountTitle = AccountSocialSecurity,
                    TransactionType = VoucherType.Salary,
                    EntryType = (byte)EntryType.SocialSecurity,
                    Credit = s.SocialSecurity.Value,
                    //Comments = "Social Security"
                    Comments = "Social Security for the month of " + month + "-" + s.Year + " against " + s.Name,

                });
            if (s.Id == 0)
            {
                var voucherNumber = new TransactionRepository().GetNextVoucherNumber(VoucherType.Salary);
                s.VoucherNumber = voucherNumber;
            }
            foreach (var item in trans)
            {
                item.VoucherNumber = s.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = s.PaymentDate ?? dt;
                item.Comments = s.Comments;
            }
            new TransactionRepository().Add(trans);
        }

        public static void DisapproveSalary(int salaryItemId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new SalaryItemRepository();
                var salaryItem = repo.GetById(salaryItemId);
                salaryItem.Status = (byte)SalaryStatus.UnApproved;
                new TransactionRepository(repo).HardDeleteByReferenceId(salaryItemId);
                repo.SaveChanges();
                scope.Complete();

            }
        }

        public static void ApproveSalary(List<int> salaryItemIds)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var esalRepo = new ESalaryRepository();
                var transRepo = new TransactionRepository(esalRepo);
                //esalRepo.ApproveBySalaryItemIds(salaryItemIds, SettingManager.SalaryExpenseId, SettingManager.CashAccountId,
                //      SettingManager.PFAccountId, SettingManager.EOBIId, SettingManager.SSTId, SettingManager.IncomeTaxId, esalRepo);

                //   public void ApproveBySalaryItemIds(List<int> salaryItemids, int salaryExpId, int CashId, int pfId, int eobiId, int sstId, int incomTaxId,ESalaryRepository repo)

                // var items = AsQueryable<SalaryItem>().Where(p => salaryItemids.Contains(p.Id));
                var salaryItemrepo = new SalaryItemRepository(esalRepo);
                var items = salaryItemrepo.GetByIds(salaryItemIds);
                var salaries = esalRepo.GetDistinctEsalaryBySalaryItems(items);
                var dt = DateTime.Now;
                var trans = new List<Transaction>();

                foreach (var item in items)
                {
                    item.Status = (byte)SalaryStatus.Approved;
                    item.ApproveBy = SiteContext.Current.User.Id;
                    item.ApproveDate = dt;
                    var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(salaries[item.ESalaryId].Month);

                    ////1. NetSalary Credit to Employee
                    //transList.Add(new Transaction()
                    //{
                    //    AccountId = item.AccountId,
                    //    AccountTitle = AccountEmloyee,
                    //    CompanyId = SiteContext.Current.User.CompanyId,
                    //    FiscalId = SiteContext.Current.Fiscal.Id,
                    //    Date = dt.Date,
                    //    TransactionType = VoucherType.Salary,
                    //    VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                    //    Credit = item.NetSalary,
                    //    CreatedDate = dt,
                    //    Comments = commentsCompany,

                    //});
                    if (item.AbsentDeduction > 0)
                    {
                        item.GrossSalary = Numerics.GetDecimal(item.GrossSalary) - Numerics.GetDecimal(item.AbsentDeduction);
                    }

                    //1. GrossSalary Debit to Salary Exp (Gross salary= Basic salary + allowance)
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.SalaryExpenseId,
                        AccountTitle = AccountSalaryExpense,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.GrossSalary,
                        CreatedDate = dt,
                        //Comments = "GrossSalary Debit To Salary Expense",
                        Comments = "GrossSalary for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                        ReferenceId = item.Id

                    });
                    //2. GrossSalary Credit to Employee (Gross salary= Basic salary + allowance)
                    trans.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        AccountTitle = "Debit To Employee",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.GrossSalary,
                        CreatedDate = dt,
                        //Comments = "GrossSalary Credit to Employee",
                        Comments = "GrossSalary for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                        ReferenceId = item.Id


                    });

                    if (item.ProvidentFund > 0)
                    {
                        //3. Provident Fund Debit to Employee
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.ProvidentFund,
                            CreatedDate = dt,
                            //Comments = "Provident Fund Debit to Employee",
                            Comments = "Provident Fund for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id
                        });
                        //4. Provident Fund Credit to PF 
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.PFAccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.ProvidentFund,
                            CreatedDate = dt,
                            //Comments = "Provident Fund Credit to PF Account",
                            Comments = "Provident Fund for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id

                        });
                        ////4. Installment Debit to Cash In Hand  (this transaction is merged to "//2. GrossSalary Debit to Salary Exp" after UA discussion)
                        //transList.Add(new Transaction()
                        //{
                        //    AccountId = CashId,
                        //    CompanyId = SiteContext.Current.User.CompanyId,
                        //    FiscalId = SiteContext.Current.Fiscal.Id,
                        //    Date = dt.Date,
                        //    TransactionType = VoucherType.Salary,
                        //    VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        //    Debit = item.Installment,
                        //    CreatedDate = dt,
                        //    Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                        //});
                    }
                    if (item.EOBI > 0)
                    {
                        //5. EOBI Debit to Employee
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.EOBI,
                            CreatedDate = dt,
                            //Comments = "EOBI Debit to Employee",
                            Comments = "EOBI for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id
                        });
                        //6. EOBI Credit to EOBI Account
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.EOBIId,
                            AccountTitle = AccountEobi,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.EOBI,
                            CreatedDate = dt,
                            //Comments = "EOBI Credit to Employee",
                            Comments = "EOBI for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id
                        });
                    }
                    if (item.SST > 0)
                    {
                        //7. SST Debit to Employee
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.SST,
                            CreatedDate = dt,
                            //Comments = "SST Debit to Employee",
                            Comments = "SST for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id
                        });
                        //8. SST Credit to SST Account
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.SSTId,
                            AccountTitle = AccountSocialSecurity,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.SST,
                            CreatedDate = dt,
                            //Comments = "SST Credit to SST Account",
                            Comments = "SST for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id

                        });
                    }
                    if (item.Bonus > 0)
                    {
                        //9. Bonus Debit to Salary Expense
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.SalaryExpenseId,
                            AccountTitle = AccountSalaryExpense,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.Bonus,
                            CreatedDate = dt,
                            //Comments = "Bonus Debit To Salary Expense",
                            Comments = "Bonus for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id

                        });
                        //10. Bonus 
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.Bonus,
                            CreatedDate = dt,
                            //Comments = "Bonus Credit to Employee",
                            Comments = "Bonus for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id


                        });
                    }
                    if (item.OTAmount > 0)
                    {
                        //11. OT Amount Debit to Salary Expense
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.SalaryExpenseId,
                            AccountTitle = AccountSalaryExpense,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.OTAmount,
                            CreatedDate = dt,
                            //Comments = "Over Time Debit To Salary Expense",
                            Comments = "Over Time for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id

                        });
                        //12. OT Amount Credit to Employee Acc
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.OTAmount,
                            CreatedDate = dt,
                            //Comments = "Over Time Credit to Employee",
                            Comments = "Over Time for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                            ReferenceId = item.Id
                        });
                    }
                    //if (item.Installment > 0)
                    //{
                    //    //13. Installment Debit to Cash In Hand
                    //    trans.Add(new Transaction()
                    //    {
                    //        AccountId = SettingManager.CashAccountId,
                    //        AccountTitle = "Cash In Hand",
                    //        CompanyId = SiteContext.Current.User.CompanyId,
                    //        FiscalId = SiteContext.Current.Fiscal.Id,
                    //        Date = dt.Date,
                    //        TransactionType = VoucherType.Salary,
                    //        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                    //        Debit = item.Installment,
                    //        CreatedDate = dt,
                    //        Comments = "Installment Debit To Cash In hand",
                    //    });
                    //    //14. Installment Credit to Employee Account
                    //    trans.Add(new Transaction()
                    //    {
                    //        AccountId = item.AccountId,
                    //        AccountTitle = AccountEmloyee,
                    //        CompanyId = SiteContext.Current.User.CompanyId,
                    //        FiscalId = SiteContext.Current.Fiscal.Id,
                    //        Date = dt.Date,
                    //        TransactionType = VoucherType.Salary,
                    //        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                    //        Credit = item.Installment,
                    //        CreatedDate = dt,
                    //        Comments = "Installment Credit to Employee",
                    //    });
                    //}

                    if (item.IncomeTax > 0)
                    {
                        //15. IncomeTax Debit to Employee
                        trans.Add(new Transaction()
                        {
                            AccountId = item.AccountId,
                            AccountTitle = AccountEmloyee,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Debit = item.IncomeTax,
                            CreatedDate = dt,
                            Comments = commentsCompany,
                            ReferenceId = item.Id
                        });
                        //16. IncomeTax Credit to IncomeTax Account
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.IncomeTaxId,
                            AccountTitle = AccountIncomeTax,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.Salary,
                            VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                            Credit = item.IncomeTax,
                            CreatedDate = dt,
                            Comments = commentsCompany,
                            ReferenceId = item.Id
                        });
                    }

                }
                transRepo.Add(trans);
                //foreach (var item in trans)
                //{
                //    transRepo.Add(item);
                //}
                esalRepo.SaveChanges();
                scope.Complete();
            }
        }

        public static void ProsessedEmailBySalaryItemIds(List<int> salaryItemIds, List<int> accountIds)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                ESalaryRepository repo = new ESalaryRepository();

                var salaryItemrepo = new SalaryItemRepository(repo);
                var accountDetailrepo = new AccountDetailRepository(repo);
                var transRepo = new TransactionRepository(repo);
                var items = salaryItemrepo.GetByIds(salaryItemIds);
                var salaries = repo.GetDistinctEsalaryBySalaryItems(items);

                var employees = accountDetailrepo.GetByAccountIds(accountIds);

                var dt = DateTime.Now;
                var transList = new List<Transaction>();
                foreach (var item in items)
                {
                    item.Status = (byte)SalaryStatus.BankProcessed;
                    item.ApproveBy = SiteContext.Current.User.Id;
                    item.ApproveDate = dt;
                    var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(salaries[item.ESalaryId].Month);

                    var employ = accountDetailrepo.GetEmployeBySalaryItem(employees, item);
                    // var employ = employees.FirstOrDefault(p => p.AccountId == item.AccountId);
                    //1. NetSalary Debit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.NetSalary,
                        CreatedDate = dt,
                        //Comments = "NetSalary Debit to Employee",
                        Comments = "NetSalary for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                    });

                    //2. NetSalary Credit to Bank
                    transList.Add(new Transaction()
                    {
                        AccountId = employ.BankId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.NetSalary,
                        CreatedDate = dt,
                        //Comments = "NetSalary Credit to Bank",
                        Comments = "NetSalary for the month of " + month + "-" + salaries[item.ESalaryId].Year + " against " + item.Name,
                    });



                }
                //foreach (var item in transList)
                //{
                //    transRepo.Add(item);
                //}
                transRepo.Add(transList);
                repo.SaveChanges();
                scope.Complete();
            }
        }
    }
}
