using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AccountDetailMap : EntityTypeConfiguration<AccountDetail>
    {
        public AccountDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(500);

            Property(t => t.FatherName)
                .HasMaxLength(500);

            Property(t => t.ContactNumber)
                .HasMaxLength(50);

            Property(t => t.ContactPerson)
                .HasMaxLength(50);

            Property(t => t.Email)
                .HasMaxLength(250);

            Property(t => t.Address)
                .HasMaxLength(1000);

            Property(t => t.BankName)
                .HasMaxLength(250);

            Property(t => t.AccountNumber)
                .HasMaxLength(50);

            Property(t => t.AccountTitle)
                .HasMaxLength(250);

            Property(t => t.GST)
                .HasMaxLength(50);

            Property(t => t.NTN)
                .HasMaxLength(50);

          

            Property(t => t.CNIC)
                .HasMaxLength(20);

            Property(t => t.BloodGroup)
                .HasMaxLength(50);

            Property(t => t.PermanentAddress)
                .HasMaxLength(1000);

            Property(t => t.Qualification)
                .HasMaxLength(250);

            //Property(t => t.Designation)
            //    .HasMaxLength(50);

            Property(t => t.PictureUrl)
                .HasMaxLength(250);

            Property(t => t.Reference)
                .HasMaxLength(1000);

            Property(t => t.PassportNumber)
                .HasMaxLength(50);

            Property(t => t.DeploymentStatus)
                .HasMaxLength(50);

            Property(t => t.Manufacturer)
                .HasMaxLength(500);

            Property(t => t.Branch)
                .HasMaxLength(250);

            Property(t => t.BranchCode)
                .HasMaxLength(50);

            Property(t => t.IBN)
                .HasMaxLength(50);

            Property(t => t.SwiftCode)
                .HasMaxLength(50);

            Property(t => t.Location)
                .HasMaxLength(500);

            Property(t => t.AssetDetail)
                .HasMaxLength(500);

            Property(t => t.Value)
                .HasMaxLength(50);

            Property(t => t.AssetType)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("AccountDetails");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AccountDetailFormId).HasColumnName("AccountDetailFormId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.FatherName).HasColumnName("FatherName");
            Property(t => t.ContactNumber).HasColumnName("ContactNumber");
            Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.BankName).HasColumnName("BankName");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.GST).HasColumnName("GST");
            Property(t => t.NTN).HasColumnName("NTN");
            Property(t => t.Others).HasColumnName("Others");
            Property(t => t.CNIC).HasColumnName("CNIC");
            Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            Property(t => t.PermanentAddress).HasColumnName("PermanentAddress");
            Property(t => t.Qualification).HasColumnName("Qualification");
            Property(t => t.Designation).HasColumnName("Designation");
            Property(t => t.PictureUrl).HasColumnName("PictureUrl");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            Property(t => t.DeploymentStatus).HasColumnName("DeploymentStatus");
            Property(t => t.DOB).HasColumnName("DOB");
            Property(t => t.DOJ).HasColumnName("DOJ");
            Property(t => t.Manufacturer).HasColumnName("Manufacturer");
            Property(t => t.PurchasePrice).HasColumnName("PurchasePrice");
            Property(t => t.SalePrice).HasColumnName("SalePrice");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.PackingPerCarton).HasColumnName("PackingPerCarton");
            Property(t => t.Branch).HasColumnName("Branch");
            Property(t => t.BranchCode).HasColumnName("BranchCode");
            Property(t => t.IBN).HasColumnName("IBN");
            Property(t => t.SwiftCode).HasColumnName("SwiftCode");
            Property(t => t.Location).HasColumnName("Location");
            Property(t => t.AssetDetail).HasColumnName("AssetDetail");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.AssetType).HasColumnName("AssetType");
            Property(t => t.AnnualAllowed).HasColumnName("AnnualAllowed");
            Property(t => t.SickAllowed).HasColumnName("SickAllowed");
            Property(t => t.CasualAllowed).HasColumnName("CasualAllowed");
            Property(t => t.CompensateryAllowed).HasColumnName("CompensateryAllowed");
        }
    }
}
