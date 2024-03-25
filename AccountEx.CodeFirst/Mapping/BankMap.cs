using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class BankMap : EntityTypeConfiguration<Bank>
    {
        public BankMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(50);

            Property(t => t.AccountNumber)
                .HasMaxLength(50);

            Property(t => t.AccountTitle)
                .HasMaxLength(250);

            Property(t => t.Address)
                .HasMaxLength(1000);

            Property(t => t.ContactNumber)
                .HasMaxLength(50);

            Property(t => t.ContactPerson)
                .HasMaxLength(1000);

            Property(t => t.Branch)
                .HasMaxLength(250);

            Property(t => t.BranchCode)
                .HasMaxLength(50);

            Property(t => t.IBN)
                .HasMaxLength(50);

            Property(t => t.SwiftCode)
                .HasMaxLength(50);

            Property(t => t.Others)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Banks");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.ContactNumber).HasColumnName("ContactNumber");
            Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            Property(t => t.Branch).HasColumnName("Branch");
            Property(t => t.BranchCode).HasColumnName("BranchCode");
            Property(t => t.IBN).HasColumnName("IBN");
            Property(t => t.SwiftCode).HasColumnName("SwiftCode");
            Property(t => t.Others).HasColumnName("Others");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
