using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ReportMap : EntityTypeConfiguration<Report>
    {
        //public ReportMap()
        //{
        //    // Primary Key
        //    HasKey(t => t.Id);

        //    // Properties
        //    Property(t => t.Location)
        //        .HasMaxLength(500);

        //    Property(t => t.Name)
        //        .HasMaxLength(500);

        //    Property(t => t.ReportType)
        //        .HasMaxLength(50);

        //    // Table & Column Mappings
        //    ToTable("Reports");
        //    Property(t => t.Id).HasColumnName("Id");
        //    Property(t => t.Location).HasColumnName("Location");
        //    Property(t => t.Name).HasColumnName("Name");
        //    Property(t => t.ReportType).HasColumnName("ReportType");
        //    Property(t => t.ShowFooter).HasColumnName("ShowFooter");
        //    Property(t => t.ShowFirstRow).HasColumnName("ShowFirstRow");
        //    Property(t => t.Order).HasColumnName("Order");
        //    Property(t => t.CompanyId).HasColumnName("CompanyId");
        //}
    }
}
