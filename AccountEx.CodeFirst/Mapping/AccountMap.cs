using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(250);

            Property(t => t.DisplayName)
                .HasMaxLength(250);

            Property(t => t.AccountCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Accounts");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountTypeId).HasColumnName("AccountTypeId");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.DisplayName).HasColumnName("DisplayName");
            Property(t => t.AccountCode).HasColumnName("AccountCode");
            Property(t => t.Level).HasColumnName("Level");
            Property(t => t.HasChild).HasColumnName("HasChild");
            Property(t => t.IsLive).HasColumnName("IsLive");
            Property(t => t.IsSystemAccount).HasColumnName("IsSystemAccount");
            Property(t => t.ReferenceId).HasColumnName("ReferenceId");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
