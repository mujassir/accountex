using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Title)
                .HasMaxLength(500);

            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            Property(t => t.PONumber)
                .HasMaxLength(10);

            Property(t => t.PictureUrl)
                .HasMaxLength(500);

            Property(t => t.WorkScope)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Projects");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Number).HasColumnName("Number");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.PONumber).HasColumnName("PONumber");
            Property(t => t.PictureUrl).HasColumnName("PictureUrl");
            Property(t => t.POIssueDate).HasColumnName("POIssueDate");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.GrossCost).HasColumnName("GrossCost");
            Property(t => t.GST).HasColumnName("GST");
            Property(t => t.GSTPercent).HasColumnName("GSTPercent");
            Property(t => t.WHT).HasColumnName("WHT");
            Property(t => t.WHTPercent).HasColumnName("WHTPercent");
            Property(t => t.Miscellaneous).HasColumnName("Miscellaneous");
            Property(t => t.NetCost).HasColumnName("NetCost");
            Property(t => t.WorkScope).HasColumnName("WorkScope");
            Property(t => t.Engineering_Planned).HasColumnName("Engineering_Planned");
            Property(t => t.Engineering_Actual).HasColumnName("Engineering_Actual");
            Property(t => t.Planning_Planned).HasColumnName("Planning_Planned");
            Property(t => t.Planning_Actual).HasColumnName("Planning_Actual");
            Property(t => t.Procurement_Planned).HasColumnName("Procurement_Planned");
            Property(t => t.Procurement_Actual).HasColumnName("Procurement_Actual");
            Property(t => t.Construction_Planned).HasColumnName("Construction_Planned");
            Property(t => t.Construction_Actual).HasColumnName("Construction_Actual");
            Property(t => t.Commissioning_Planned).HasColumnName("Commissioning_Planned");
            Property(t => t.Commissioning_Actual).HasColumnName("Commissioning_Actual");
            Property(t => t.HandOver_Planned).HasColumnName("HandOver_Planned");
            Property(t => t.HandOver_Actual).HasColumnName("HandOver_Actual");
            Property(t => t.Manpower_Planned).HasColumnName("Manpower_Planned");
            Property(t => t.Manpower_Actual).HasColumnName("Manpower_Actual");
            Property(t => t.DirectMaterial_Planned).HasColumnName("DirectMaterial_Planned");
            Property(t => t.DirectMaterial_Actual).HasColumnName("DirectMaterial_Actual");
            Property(t => t.Transportation_Planned).HasColumnName("Transportation_Planned");
            Property(t => t.Transportation_Actual).HasColumnName("Transportation_Actual");
            Property(t => t.ToolnEquipment_Planned).HasColumnName("ToolnEquipment_Planned");
            Property(t => t.ToolnEquipment_Actual).HasColumnName("ToolnEquipment_Actual");
            Property(t => t.SiteOfficeCost_Planned).HasColumnName("SiteOfficeCost_Planned");
            Property(t => t.SiteOfficeCost_Actual).HasColumnName("SiteOfficeCost_Actual");
            Property(t => t.Misc_Planned).HasColumnName("Misc_Planned");
            Property(t => t.Misc_Actual).HasColumnName("Misc_Actual");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
