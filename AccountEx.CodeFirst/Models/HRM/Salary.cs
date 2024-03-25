using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Salary : BaseEntity
    {
        
        public int AccountId { get; set; }
        public int VoucherNumber { get; set; }
        public int Month { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public Nullable<decimal> MedicalSecurity { get; set; }
        public Nullable<decimal> EOBI { get; set; }
        public Nullable<decimal> Insurance { get; set; }
        public Nullable<decimal> ProvidentFund { get; set; }
        public Nullable<decimal> TotalBD { get; set; }
        public Nullable<decimal> AdvanceBalance { get; set; }
        public Nullable<decimal> Installment { get; set; }
        public Nullable<decimal> NetBalance { get; set; }
        public Nullable<decimal> HouseRent { get; set; }
        public Nullable<decimal> Medical { get; set; }
        public Nullable<decimal> Etc { get; set; }
        public Nullable<decimal> TotalAllowances { get; set; }
        public Nullable<int> OTTotalWorkingDays { get; set; }
        public Nullable<decimal> OTPerHourCost { get; set; }
        public Nullable<decimal> OTHours { get; set; }
        public Nullable<decimal> OTHourlyRate { get; set; }
        public Nullable<decimal> OTCost { get; set; }
        public Nullable<decimal> GHHours { get; set; }
        public Nullable<decimal> GHHourlyRate { get; set; }
        public Nullable<decimal> GHCost { get; set; }
        public Nullable<decimal> SOTHours { get; set; }
        public Nullable<decimal> SOTHourlyRate { get; set; }
        public Nullable<decimal> SOTCost { get; set; }
        public Nullable<decimal> TotalOTCost { get; set; }
        public Nullable<decimal> SummaryAllowances { get; set; }
        public Nullable<decimal> SummaryOT { get; set; }
        public Nullable<decimal> SummaryDeductions { get; set; }
        public Nullable<decimal> SummarySalary { get; set; }
        public Nullable<decimal> IncomeTax { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public string Comments { get; set; }
        public Nullable<decimal> Absents { get; set; }
        public Nullable<decimal> AbsentsCost { get; set; }
        public Nullable<decimal> CasualAllowed { get; set; }
        public Nullable<decimal> CasualAvailed { get; set; }
        public Nullable<decimal> CasualBalance { get; set; }
        public Nullable<decimal> SickAllowed { get; set; }
        public Nullable<decimal> SickAvailed { get; set; }
        public Nullable<decimal> SickBalance { get; set; }
        public Nullable<decimal> AnnualAllowed { get; set; }
        public Nullable<decimal> AnnualAvailed { get; set; }
        public Nullable<decimal> AnnualBalance { get; set; }
        public Nullable<decimal> CompensateryAllowed { get; set; }
        public Nullable<decimal> CompensateryAvailed { get; set; }
        public Nullable<decimal> CompensateryBalance { get; set; }
        
        public bool IsProcessed { get; set; }
        public Nullable<decimal> Conveyance { get; set; }
        public Nullable<decimal> SocialSecurity { get; set; }
        public Nullable<int> Year { get; set; }
       
        public Nullable<DateTime> Date { get; set; }
        public Nullable<int> OldId { get; set; }

        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public Nullable<decimal> WorkingDays { get; set; }
        public Nullable<decimal> GrossSalary { get; set; }

    }
}
