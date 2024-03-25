using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class SalaryMap : EntityTypeConfiguration<Salary>
    {
        public SalaryMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(50);

            Property(t => t.Comments)
                .HasMaxLength(1000);

            // Table & Column Mappings
            ToTable("Salaries");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Month).HasColumnName("Month");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            Property(t => t.MedicalSecurity).HasColumnName("MedicalSecurity");
            Property(t => t.EOBI).HasColumnName("EOBI");
            Property(t => t.Insurance).HasColumnName("Insurance");
            Property(t => t.ProvidentFund).HasColumnName("ProvidentFund");
            Property(t => t.TotalBD).HasColumnName("TotalBD");
            Property(t => t.AdvanceBalance).HasColumnName("AdvanceBalance");
            Property(t => t.Installment).HasColumnName("Installment");
            Property(t => t.NetBalance).HasColumnName("NetBalance");
            Property(t => t.HouseRent).HasColumnName("HouseRent");
            Property(t => t.Medical).HasColumnName("Medical");
            Property(t => t.Etc).HasColumnName("Etc");
            Property(t => t.TotalAllowances).HasColumnName("TotalAllowances");
            Property(t => t.OTTotalWorkingDays).HasColumnName("OTTotalWorkingDays");
            Property(t => t.OTPerHourCost).HasColumnName("OTPerHourCost");
            Property(t => t.OTHours).HasColumnName("OTHours");
            Property(t => t.OTHourlyRate).HasColumnName("OTHourlyRate");
            Property(t => t.OTCost).HasColumnName("OTCost");
            Property(t => t.GHHours).HasColumnName("GHHours");
            Property(t => t.GHHourlyRate).HasColumnName("GHHourlyRate");
            Property(t => t.GHCost).HasColumnName("GHCost");
            Property(t => t.SOTHours).HasColumnName("SOTHours");
            Property(t => t.SOTHourlyRate).HasColumnName("SOTHourlyRate");
            Property(t => t.SOTCost).HasColumnName("SOTCost");
            Property(t => t.TotalOTCost).HasColumnName("TotalOTCost");
            Property(t => t.SummaryAllowances).HasColumnName("SummaryAllowances");
            Property(t => t.SummaryOT).HasColumnName("SummaryOT");
            Property(t => t.SummaryDeductions).HasColumnName("SummaryDeductions");
            Property(t => t.SummarySalary).HasColumnName("SummarySalary");
            Property(t => t.IncomeTax).HasColumnName("IncomeTax");
            Property(t => t.NetSalary).HasColumnName("NetSalary");
            Property(t => t.PaymentDate).HasColumnName("PaymentDate");
            Property(t => t.Comments).HasColumnName("Comments");
            Property(t => t.Absents).HasColumnName("Absents");
            Property(t => t.AbsentsCost).HasColumnName("AbsentsCost");
            Property(t => t.CasualAllowed).HasColumnName("CasualAllowed");
            Property(t => t.CasualAvailed).HasColumnName("CasualAvailed");
            Property(t => t.CasualBalance).HasColumnName("CasualBalance");
            Property(t => t.SickAllowed).HasColumnName("SickAllowed");
            Property(t => t.SickAvailed).HasColumnName("SickAvailed");
            Property(t => t.SickBalance).HasColumnName("SickBalance");
            Property(t => t.AnnualAllowed).HasColumnName("AnnualAllowed");
            Property(t => t.AnnualAvailed).HasColumnName("AnnualAvailed");
            Property(t => t.AnnualBalance).HasColumnName("AnnualBalance");
            Property(t => t.CompensateryAllowed).HasColumnName("CompensateryAllowed");
            Property(t => t.CompensateryAvailed).HasColumnName("CompensateryAvailed");
            Property(t => t.CompensateryBalance).HasColumnName("CompensateryBalance");
            Property(t => t.IsProcessed).HasColumnName("IsProcessed");
            Property(t => t.Conveyance).HasColumnName("Conveyance");
            Property(t => t.SocialSecurity).HasColumnName("SocialSecurity");
            Property(t => t.Year).HasColumnName("Year");
        }
    }
}
