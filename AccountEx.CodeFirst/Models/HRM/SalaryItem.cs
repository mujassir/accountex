using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace AccountEx.CodeFirst.Models
{
    public partial class SalaryItem : BaseEntity
    {
        public int ESalaryId { get; set; }
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string Name { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal EOBI { get; set; }
        public decimal SST { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NoOfAbsents { get; set; }
        public decimal OverTimeRate { get; set; }
        public Nullable<decimal> AbsentDeduction { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Installment { get; set; }
        public decimal IncomeTax { get; set; }
        public int WorkingDays { get; set; }
        public int OTHours { get; set; }
        public decimal OTAmount { get; set; }
        public decimal Bonus { get; set; }
        public decimal NetSalary { get; set; }
        public byte Status { get; set; }
        public Nullable<int> ApproveBy { get; set; }
        public Nullable<DateTime> ApproveDate { get; set; }


        [JsonIgnore]
        public virtual Department Department { get; set; }
        [JsonIgnore]
        public virtual Designation Designation { get; set; }

        [NotMapped]
        public string DepartmentName { get {  return Department != null ? Department.Name : "";  }  }

        [NotMapped]
        public string DesignationName { get { return Designation != null ? Designation.Name : ""; } }
    }
}