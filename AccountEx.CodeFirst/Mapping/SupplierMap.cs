using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class SupplierMap : EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(50);

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

            Property(t => t.Others)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Suppliers");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
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
        }
    }
}
