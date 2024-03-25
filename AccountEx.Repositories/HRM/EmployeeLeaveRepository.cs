using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class EmployeeLeaveRepository : GenericRepository<EmployeeLeave>
    {
        public List<EmployeeLeave> GetLeavesByDate(DateTime fromdate, DateTime todate)
        {
            return Collection.Where(p => p.FromDate == fromdate && p.ToDate == todate).ToList();
        }
        public void DeleteByDateRange(DateTime fromdate, DateTime todate,string name)
        {
            //var fromDate = fromdate.ToString("MM dd, yyyy");
            //var todate = todate.ToString("MM dd yyyy");
            var query = "Delete from EmployeeLeaves where Name='" + name + "' and CompanyId='" + SiteContext.Current.User.CompanyId + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
        public EmployeeLeave GetByName(string name)
        {
            name = name.ToLower();
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
        public List<EmployeeLeave> GetEmployeeleaveByMonth(int month, int year)
        {
            return Collection.Where(p => p.Month == month && p.Year == year && p.CompanyId==SiteContext.Current.User.CompanyId).ToList();
        }
        public bool CheckEmployeeleaveByMonth(int month, int year)
        {
            return Collection.Any(p => p.Month == month && p.Year == year && p.CompanyId == SiteContext.Current.User.CompanyId);
        }
        public void DeleteByMonth(int month, int year, string name)
        {
            var query = "Delete from EmployeeLeaves where [Month]='" + month + "' and [Year]='" + year + "' and CompanyId='" + SiteContext.Current.User.CompanyId + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
        
    }
}
