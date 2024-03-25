using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class EmployeeIncomeConfigRepository : GenericRepository<EmployeeIncomeConfig>
    {
        public List<EmployeeIncomeConfig> GetEmployeeIncomeByDate(DateTime fromdate,DateTime todate)
        {
            return Collection.Where(p => p.FromDate == fromdate && p.ToDate == todate).ToList();
        }
        public void DeleteByDateRange(DateTime fromdate, DateTime todate)
        {
            //var fromDate = fromdate.ToString("MM dd, yyyy");
            //var todate = todate.ToString("MM dd yyyy");
            var query = "Delete from EmployeeIncomeConfigs where FromDate='" + fromdate.ToString("MM/dd/yyyy") + "' and ToDate='" + todate.ToString("MM/dd/yyyy") + "'";
            Db.Database.ExecuteSqlCommand(query);
        }
        public EmployeeIncomeConfig GetByName(string name)
        {
            name = name.ToLower();
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
        //public void Save(EmployeeIncomeConfigExtra input)     
        //{
        //        foreach (var emp in input.EmployeeIncomeList)
        //        {
        //           emp.FromDate=DateTime.Now;
        //        }
        //        base.Save(input.EmployeeIncomeList);
        //        updatePreviousRecord();
        //}
        //private void updatePreviousRecord()
        //{
        //    foreach (var item in Collection.Where(p => p.ToDate == null).ToList())
        //    {
        //        item.ToDate = DateTime.Now;
        //    }
        //    Db.SaveChanges();
        //}

        public List<EmployeeIncomeConfig> GetEmployeeIncomeByMonth(int month, int Year)
        {
            DateTime date = new DateTime(Year, month, 1);
            return Collection.Where(p => p.FromDate <= date && p.ToDate >= date).ToList();
        }
       
    }
}
