using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class FormSettingMap : EntityTypeConfiguration<FormSetting>
    {
        public FormSettingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.VoucherType)
                .IsRequired()
                .HasMaxLength(500);

            Property(t => t.KeyName)
                .IsRequired()
                .HasMaxLength(500);

            Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(500);

            Property(t => t.AccountTitle)
                .HasMaxLength(250);

            // Table & Column Mappings
            ToTable("FormSettings");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.VoucherType).HasColumnName("VoucherType");
            Property(t => t.KeyName).HasColumnName("KeyName");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.UseCOA).HasColumnName("UseCOA");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
