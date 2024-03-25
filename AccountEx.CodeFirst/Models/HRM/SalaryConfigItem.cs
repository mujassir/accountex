using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst
{

    public partial class SalaryConfigItem : BaseEntity
    {
        /// <summary>
        /// Foreign key to SalaryConfig
        /// </summary>
        public int SalaryConfigId { get; set; }
        /// <summary>
        /// AccountId represents Id in Accounts table for Employees
        /// </summary>
        public int AccountId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal EOBI { get; set; }
        public decimal SST { get; set; }
        public decimal OverTimeRate { get; set; }
        public int AnnualLeaves { get; set; }
        public int CasualLeaves { get; set; }
        public int SickLeaves { get; set; }
        public int WorkingDays { get; set; }
        public bool IsActive { get; set; }

    }
}
