using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Key)
                .HasMaxLength(500);

            Property(t => t.Value)
                .HasMaxLength(1000);

            // Table & Column Mappings
            ToTable("Settings");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.Key).HasColumnName("Key");
            Property(t => t.Value).HasColumnName("Value");
        }
    }
}
