using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class SalaryExtra : Salary
    {
        public List<Salary> EmployeeSalaryList { get; set; }
        public string RecordType { get; set; }
    }
}
