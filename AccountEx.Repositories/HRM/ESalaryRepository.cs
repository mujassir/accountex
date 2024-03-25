using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System;

namespace AccountEx.Repositories
{
    public class ESalaryRepository : GenericRepository<ESalary>
    {
        public ESalaryRepository() : base() { }
        public ESalaryRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public ESalary GetByVoucherNumber(int voucherNumber)
        {
            return Collection.FirstOrDefault(p => p.VoucherNumber == voucherNumber);
        }

        public List<ESalary> GetESalaryByMonthYear(int month, int Year)
        {
            return Collection.Where(p => p.Month == month && p.Year == Year).ToList();
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!Collection.Any())
                return maxnumber;
            return Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public List<SalaryCalculation> GetSalaryConfigItems(DateTime date)
        {
            var query = string.Format("EXEC dbo.GetSalaryItems @Date = '{0}', @CompanyId = {1}",
                                     date.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<SalaryCalculation>(query).ToList();
        }
        public List<ESalary> GetByDate(DateTime date)
        {
            return Collection.Where(p => p.Month == date.Month && p.Year == date.Year).ToList();
        }

        public List<ESalary> GetMailSalaryByDate(DateTime date)
        {
            return Collection.Where(p => p.Month == date.Month && p.Year == date.Year).ToList();
        }
        public List<ESalary> GetUnApproveSalaryByDate(DateTime date)
        {
            return Collection.Where(p => p.Month == date.Month && p.Year == date.Year && p.SalaryItems.Any(c => c.Status == (byte)SalaryStatus.UnApproved)).ToList();
        }

        public List<ESalary> GetApproveSalaryByDate(DateTime date)
        {
            return Collection.Where(p => p.Month == date.Month && p.Year == date.Year && p.SalaryItems.Any(c => c.Status == (byte)SalaryStatus.Approved)).ToList();
        }

        public override void Save(ESalary entity)
        {
            foreach (var item in entity.SalaryItems)
            {
                var salary = item.BasicSalary + item.HouseAllowance + item.ConveyanceAllowance + item.MedicalAllowance;
                if (item.ProvidentFund < 1) item.ProvidentFund = item.ProvidentFund * salary;
                if (item.SST < 1) item.SST = item.SST * salary;
                if (item.EOBI < 1) item.EOBI = item.EOBI * salary;
            }
            base.Save(entity);
        }

        public Dictionary<int, ESalary> GetDistinctEsalaryBySalaryItems(List<SalaryItem> salaryItems)
        {
            var salaryIds = salaryItems.Select(p => p.ESalaryId).Distinct().ToList();
            var items=Collection.Where(p => salaryIds.Contains(p.Id)).ToDictionary(q => q.Id, p => p);
            return items;

        }

  
        public void ApproveBySalaryItemIds(List<int> salaryItemids, int salaryExpId, int CashId, int pfId, int eobiId, int sstId, int incomTaxId,ESalaryRepository repo)
        {
           
           // var items = AsQueryable<SalaryItem>().Where(p => salaryItemids.Contains(p.Id));
            var salaryItemrepo = new SalaryItemRepository(repo);
            var items= salaryItemrepo.GetByIds(salaryItemids);
            var salaryIds = items.Select(p => p.ESalaryId).Distinct();
            var salaries = Collection.Where(p => salaryIds.Contains(p.Id)).ToDictionary(q => q.Id, p => p);
            var dt = DateTime.Now;
            var transList = new List<Transaction>();
            foreach (var item in items)
            {
                item.Status = (byte)SalaryStatus.Approved;
                item.ApproveBy = SiteContext.Current.User.Id;
                item.ApproveDate = dt;

                //1. NetSalary Credit to Employee
                transList.Add(new Transaction()
                 {
                     AccountId = item.AccountId,
                     CompanyId = SiteContext.Current.User.CompanyId,
                     FiscalId = SiteContext.Current.Fiscal.Id,
                     Date = dt.Date,
                     TransactionType = VoucherType.Salary,
                     VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                     Credit = item.NetSalary,
                     CreatedDate = dt,
                     Comments = "Salary Approved by " + SiteContext.Current.User.Username,

                 });

                //2. GrossSalary Debit to Salary Exp
                transList.Add(new Transaction()
                {
                    AccountId = salaryExpId,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Date = dt.Date,
                    TransactionType = VoucherType.Salary,
                    VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                    Debit = item.GrossSalary,
                    CreatedDate = dt,
                    Comments = "Salary Approved by " + SiteContext.Current.User.Username,

                });
                if (item.Installment > 0)
                {
                    //3. Installment Credit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.Installment,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
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
                if (item.ProvidentFund > 0)
                {
                    //5. ProvidentFund Debit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.ProvidentFund,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                    //6. ProvidentFund Credit to PF Account
                    transList.Add(new Transaction()
                    {
                        AccountId = pfId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.ProvidentFund,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                }
                if (item.SST > 0)
                {
                    //7. SST Debit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.SST,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                    //8. SST Credit to SST Account
                    transList.Add(new Transaction()
                    {
                        AccountId = sstId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.SST,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                }
                if (item.EOBI > 0)
                {
                    //9. EOBI Debit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.EOBI,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                    //10. EOBI Credit to EOBI Account
                    transList.Add(new Transaction()
                    {
                        AccountId = eobiId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.EOBI,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                }
                if (item.IncomeTax > 0)
                {
                    //11. IncomeTax Debit to Employee
                    transList.Add(new Transaction()
                    {
                        AccountId = item.AccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Debit = item.IncomeTax,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                    //12. IncomeTax Credit to IncomeTax Account
                    transList.Add(new Transaction()
                    {
                        AccountId = incomTaxId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.Salary,
                        VoucherNumber = salaries[item.ESalaryId].VoucherNumber,
                        Credit = item.IncomeTax,
                        CreatedDate = dt,
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                }

            }
            foreach (var item in transList)
            {
                Db.Transactions.Add(item);
            }
            Db.SaveChanges();
        }


        public void ProsessedEmailBySalaryItemIds(List<int> salaryItemids, List<int> accountIds, ESalaryRepository repo)
        {
            //var items = AsQueryable<SalaryItem>().Where(p => salaryItemids.Contains(p.Id));
            var salaryItemrepo = new SalaryItemRepository(repo);
            var accountDetailrepo = new AccountDetailRepository(repo);
            var items = salaryItemrepo.GetByIds(salaryItemids);
            var salaryIds = items.Select(p => p.ESalaryId).Distinct();
            var salaries = Collection.Where(p => salaryIds.Contains(p.Id)).ToDictionary(q => q.Id, p => p);

            var employees = accountDetailrepo.GetByAccountIds(accountIds);

            var dt = DateTime.Now;
            var transList = new List<Transaction>();
            foreach (var item in items)
            {
                item.Status = (byte)SalaryStatus.BankProcessed;
                item.ApproveBy = SiteContext.Current.User.Id;
                item.ApproveDate = dt;

                var employ = employees.FirstOrDefault(p => p.AccountId == item.AccountId);
                //1. NetSalary Credit to Employee
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
                    Comments = "Processed Salary by " + SiteContext.Current.User.Username,

                });
                  
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
                        Comments = "Salary Approved by " + SiteContext.Current.User.Username,
                    });
                  
                

            }
            foreach (var item in transList)
            {
                Db.Transactions.Add(item);
            }
            Db.SaveChanges();
        }
    
        public void SendEmail(DateTime date)
        {
            var repo = new AccountDetailRepository();

            var salary = Collection.Where(p => p.Month == date.Month && p.Year == date.Year).ToList();
            var accounIds=salary.SelectMany(p=>p.SalaryItems).Select(p=>p.AccountId).Distinct().ToList();
            var employees = repo.GetByAccountIds(accounIds);

            var bankIds = employees.Select(p => p.BankId).Distinct().ToList();
            var banks = repo.GetByAccountIds(accounIds);
            foreach (var item in banks)
            {
                var bankEmployees = employees.Where(p => p.BankId == item.AccountId).ToList();
            }
            //.Select(p => p.SalaryItems);

            //var data = Collection.Where(p => p.Month == date.Month && p.Year == date.Year)        // source  bbr  FK
            //.Join(AsQueryable<AccountDetail>(),         // target 
            //  si => si.SalaryItems         // FK
            //   ad => ad.AccountId,   // PK
            //   (si, ad) => new { SalaryItem = si, AccountDetail = ad }) // project result
            //   .Where
            //.Select(x => new
            //{
            //    SalaryItemId = x.SalaryItem.Id,
            //    BankName = x.Bank.Name,
            //    BanchId = x.BankBranch.Id,
            //    BranchName = x.BankBranch.Name,

            //});
        }

        public void UpdateSalaryStatus(List<ESalary> esalaries)
        {
            foreach (var esal in esalaries)
            {
                foreach (var salaryitem in esal.SalaryItems)
                {
                    salaryitem.Status = (byte)SalaryStatus.EmailToBank;
                }
            }
            Db.SaveChanges();
        }
        public List<SalaryItem> UpdateSalaryStatus(List<int> salaryItemids)
        {
            var salaryItems = AsQueryable<SalaryItem>().Where(p => salaryItemids.Contains(p.Id)).ToList();
            var salaryIds = salaryItems.Select(p => p.ESalaryId).Distinct();
            var salaries = Collection.Where(p => salaryIds.Contains(p.Id)).ToDictionary(q => q.Id, p => p);
            var dt = DateTime.Now;
            var transList = new List<Transaction>();
            foreach (var item in salaryItems)
            {
                item.Status = (byte)SalaryStatus.EmailToBank;
                item.ApproveBy = SiteContext.Current.User.Id;
                item.ApproveDate = dt;

            }
            Db.SaveChanges();
            return salaryItems;
        }

      

        public void DeleteSalaryItems(List<int> salaryItemids)
        {
            var salaryItems = Db.SalaryItems.Where(p => salaryItemids.Contains(p.Id) && p.CompanyId == SiteContext.Current.User.CompanyId).ToList();
            foreach (var item in salaryItems)
            {
                Db.SalaryItems.Remove(item);
            }
            Db.SaveChanges();
        }
    }
}
