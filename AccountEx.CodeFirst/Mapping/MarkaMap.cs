using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class MarkaMap : EntityTypeConfiguration<Marka>
    {
        public MarkaMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Markas");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
