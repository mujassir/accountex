using System.Collections.Generic;
using System;

namespace AccountEx.Common
{
    public class SalaryCalculation 
    {
        public int SalaryConfigId { get; set; }
        /// <summary>
        /// AccountId represents Id in Accounts table for Employees
        /// </summary>
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string Name { get; set; }
        public int VoucherNumber { get; set; }

        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> DesignationId { get; set; }

        public string  DepartmentName { get; set; }
        public string  DesignationName { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        /// <summary>
        /// BasicSalary + TotalAllowances
        /// </summary>
        public decimal Salary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal EOBI { get; set; }
        public decimal SST { get; set; }
        public int OTHours { get; set; }
        public decimal OverTimeRate { get; set; }
        public decimal OTAmount { get; set; }
        public int NoOfAbsents { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal Installment { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetSalary { get; set; }
        public int AnnualLeaves { get; set; }
        public int CasualLeaves { get; set; }
        public int SickLeaves { get; set; }
        public int WorkingDays { get; set; }       
    }
}
