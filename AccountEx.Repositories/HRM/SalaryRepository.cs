using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using System;

namespace AccountEx.Repositories
{
    public class SalaryRepository : GenericRepository<Salary>
    {
         public SalaryRepository() : base() { }
         public SalaryRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Salary> GetByMonthYear(bool isProcessed, int month, int year)
        {
            return Collection.Where(p => p.IsProcessed == isProcessed && p.Month == month).ToList();
        }
        public List<Salary> GetByMonthYear(int month, int year)
        {
            return Collection.Where(p => p.Month == month && p.Year == year).ToList();
        }
        public Salary GetLeaveRecord(int employeeId, int year)
        {
            var record = Collection.Where(p => p.IsProcessed && p.Year == year && p.AccountId == employeeId).GroupBy(p => new { p.AccountId, p.Year }).Select(p => new
            {
                CasualAvailed = p.Sum(q => q.CasualAvailed),
                SickAvailed = p.Sum(q => q.SickAvailed),
                AnnualAllowed = p.Sum(q => q.AnnualAvailed),
                CompensateryAvailed = p.Sum(q => q.CompensateryAvailed)
            }).ToList().Select(p => new Salary()
            {
                CasualAvailed = p.CasualAvailed ?? 0,
                SickAvailed = p.SickAvailed ?? 0,
                AnnualAvailed = p.AnnualAllowed ?? 0,
                CompensateryAvailed = p.CompensateryAvailed ?? 0,
            }).FirstOrDefault();
            return record;
        }
        public Salary GetByVoucherNumber(int voucherNumber)
        {
            return Collection.FirstOrDefault(p => p.VoucherNumber == voucherNumber);
        }
        public override void Delete(int id)
        {
            var salary = Db.Salaries.FirstOrDefault(p => p.Id == id);
            if (salary != null)
            {
                var salaryTransactions = Db.Transactions.Where(p => p.CompanyId == SiteContext.Current.User.CompanyId
                    && p.TransactionType  == VoucherType.Salary && p.VoucherNumber == salary.VoucherNumber).ToList() ;

                foreach (var item in salaryTransactions)
                {
                    Db.Transactions.Remove(item);
                }
                Db.Salaries.Remove(salary);
                Db.SaveChanges();
            }
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            //if (!Collection.Any(p => p.TransactionType == vouchertype && !p.IsDeleted))
            //    return maxnumber;
            return Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public List<Salary> GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            List<Salary> v = null;
            switch (key)
            {
                case "first":
                    //  v = Collection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    var firstvoucherno = Collection.Where(p => p.VoucherNumber == voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault().VoucherNumber;
                    v = Collection.Where(p => p.VoucherNumber == firstvoucherno).OrderBy(p => p.VoucherNumber).ToList();
                    break;
                case "last":
                    //    v = Collection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    var lastvoucherno = Collection.Where(p => p.VoucherNumber == voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber;
                    v = Collection.Where(p => p.VoucherNumber == lastvoucherno).OrderByDescending(p => p.VoucherNumber).ToList();
                    break;
                case "next":
                   // v = Collection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    var nextvoucherno = Collection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault().VoucherNumber;
                    v = Collection.Where(p => p.VoucherNumber == nextvoucherno).OrderBy(p => p.VoucherNumber).ToList();
                    break;
                case "previous":
                  //  v = Collection.Where(p =>  p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    var prevoucherno = Collection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber;
                    v = Collection.Where(p => p.VoucherNumber == prevoucherno).OrderByDescending(p => p.VoucherNumber).ToList();
                    break;
                case "same":
                    v = Collection.Where(p => p.VoucherNumber == voucherno).ToList();
                    break;
                case "challan":
                    v = Collection.Where(p => p.VoucherNumber == voucherno).ToList();
                    break;

            }

            //if (v != null)
            //    voucherno = v.VoucherNumber;
            //else if (key != "nextvouchernumber" )
            //{
            //    v = Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            //}
            //if (v == null )
            //{
            //    v = new Salary();
            //    v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            //    v.Date = DateTime.Now;
            //   // v.CreatedDate = DateTime.Now;
            //}
            next = Collection.Any(p => p.VoucherNumber > voucherno);
            previous = Collection.Any(p =>  p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {
            foreach (var item in Collection.Where(p => p.VoucherNumber == voucherno ).ToList())
            {
                Db.Salaries.Remove(item);
            }
            Db.SaveChanges();
        }
        public void DeleteByVoucherNumberHardDelete(string voucherno)
        {
            var query = "Delete from Salaries where VoucherNumber='" + voucherno + "' and CompanyId='" + SiteContext.Current.UserCompany + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
    }
}
