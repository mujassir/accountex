using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ConfigMap : EntityTypeConfiguration<Configuration>
    {
        public ConfigMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Key)
                .HasMaxLength(100);

            Property(t => t.Value)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Configs");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Key).HasColumnName("Key");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
