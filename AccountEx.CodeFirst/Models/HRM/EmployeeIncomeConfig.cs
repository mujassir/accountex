using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class EmployeeIncomeConfig : BaseEntity
    {
        public string Name { get; set; }
        public int AccountId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal EOBI { get; set; }
        public decimal SST { get; set; }
        public Nullable<decimal> OverTimeRate { get; set; }
        public Nullable<int> AnnualLeaves { get; set; }
        public Nullable<int> CasualLeaves { get; set; }
        public Nullable<int> SickLeaves { get; set; }
        public Nullable<int> WorkingDays { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<System.DateTime> LeaveMonthStart { get; set; }
        public Nullable<System.DateTime> LeaveBankEnd { get; set; }
    }
}
