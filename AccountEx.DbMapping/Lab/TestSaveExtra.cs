using AccountEx.CodeFirst.Models.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AccountEx.DbMapping.Lab
{
    public class TestSaveExtra
    {
        public TestSaveExtra()
        {
            TestDepartments = new List<TestDepartmentExtra>();
        }
        public Test Test { get; set; }
        public List<TestDepartmentExtra> TestDepartments { get; set; }
    }
    public class TestDepartmentExtra
    {

        public int DepartmentAccountId { get; set; }
        public string DepartmentName { get; set; }
        public decimal Price { get; set; }
    }
    public class LatestPriceExtra
    {
        public int DepartmentAccountId { get; set; }
        public decimal Price { get; set; }
    }
}
