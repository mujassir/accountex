using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ReportColumnMap : EntityTypeConfiguration<ReportColumn>
    {
        public ReportColumnMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(50);

            Property(t => t.HeaderText)
                .HasMaxLength(50);

            Property(t => t.FooterText)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ReportColumns");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReportId).HasColumnName("ReportId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.HeaderText).HasColumnName("HeaderText");
            Property(t => t.ShowSum).HasColumnName("ShowSum");
            Property(t => t.FooterText).HasColumnName("FooterText");
            Property(t => t.Colspan).HasColumnName("Colspan");
        }
    }
}
