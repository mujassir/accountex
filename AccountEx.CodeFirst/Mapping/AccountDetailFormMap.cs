using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AccountDetailFormMap : EntityTypeConfiguration<AccountDetailForm>
    {
        public AccountDetailFormMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .HasMaxLength(250);

            // Table & Column Mappings
            ToTable("AccountDetailForms");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.HeadAccountId).HasColumnName("HeadAccountId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
