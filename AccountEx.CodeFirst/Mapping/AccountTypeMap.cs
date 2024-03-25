using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AccountTypeMap : EntityTypeConfiguration<AccountType>
    {
        public AccountTypeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(50);

           

            // Table & Column Mappings
            ToTable("AccountTypes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
