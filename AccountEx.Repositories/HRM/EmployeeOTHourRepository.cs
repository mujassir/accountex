using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class EmployeeOTHourRepository : GenericRepository<EmployeeOTHour>
    {
        public List<EmployeeOTHour> GetOTByDate(DateTime fromdate, DateTime todate)
        {
            return Collection.Where(p => p.FromDate == fromdate && p.ToDate == todate).ToList();
        }
        public void DeleteByName(string name)
        {
            var query = "Delete from EmployeeOTHours where Name='" + name + "' and CompanyId='" + SiteContext.Current.User.CompanyId + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
         public void DeleteByMonth(int month,int year,string name)
        {
            var query="Delete from EmployeeOTHours where [Month]='" + month + "' and [Year]='" + year + "' and CompanyId='" + SiteContext.Current.User.CompanyId + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
        public EmployeeOTHour GetByName(string name)
        {
            name = name.ToLower();
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
        public List<EmployeeOTHour> GetEmployeeOTByMonth(int month, int year)
        {
            return Collection.Where(p => p.Month == month && p.Year == year).ToList();
        }
        public bool CheckEmployeeOTByMonth(int month, int year)
        {
            return Collection.Any(p => p.Month == month && p.Year == year);
        }
    }
}
