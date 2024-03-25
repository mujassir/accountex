using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AccountAttributeMap : EntityTypeConfiguration<AccountAttribute>
    {
        public AccountAttributeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.AttributeName)
                .HasMaxLength(500);

            Property(t => t.Value)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("AccountAttributes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AttributeId).HasColumnName("AttributeId");
            Property(t => t.AttributeName).HasColumnName("AttributeName");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
