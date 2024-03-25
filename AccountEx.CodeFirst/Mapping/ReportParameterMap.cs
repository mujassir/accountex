using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ReportParameterMap : EntityTypeConfiguration<ReportParameter>
    {
        //public ReportParameterMap()
        //{
        //    // Primary Key
        //    HasKey(t => t.Id);

        //    // Properties
        //    Property(t => t.Name)
        //        .HasMaxLength(50);

        //    Property(t => t.DisplayName)
        //        .HasMaxLength(50);

        //    Property(t => t.Type)
        //        .HasMaxLength(50);

        //    Property(t => t.SelectionClass)
        //        .HasMaxLength(500);

        //    Property(t => t.SelectionMethod)
        //        .HasMaxLength(500);

        //    Property(t => t.ValueField)
        //        .HasMaxLength(50);

        //    Property(t => t.DisplayField)
        //        .HasMaxLength(50);

        //    // Table & Column Mappings
        //    ToTable("ReportParameters");
        //    Property(t => t.Id).HasColumnName("Id");
        //    Property(t => t.ReportId).HasColumnName("ReportId");
        //    Property(t => t.Name).HasColumnName("Name");
        //    Property(t => t.DisplayName).HasColumnName("DisplayName");
        //    Property(t => t.Type).HasColumnName("Type");
        //    Property(t => t.SelectionClass).HasColumnName("SelectionClass");
        //    Property(t => t.SelectionMethod).HasColumnName("SelectionMethod");
        //    Property(t => t.ValueField).HasColumnName("ValueField");
        //    Property(t => t.DisplayField).HasColumnName("DisplayField");
        //    Property(t => t.CompanyId).HasColumnName("CompanyId");
        //}
    }
}
