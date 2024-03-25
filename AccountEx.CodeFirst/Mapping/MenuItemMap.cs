using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class MenuItemMap : EntityTypeConfiguration<MenuItem>
    {
        public MenuItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Title)
                .HasMaxLength(50);

            Property(t => t.Url)
                .HasMaxLength(250);

            Property(t => t.IconClass)
                .HasMaxLength(50);

            Property(t => t.DataType)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("MenuItems");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ParentMenuItemId).HasColumnName("ParentMenuItemId");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.Url).HasColumnName("Url");
            Property(t => t.IconClass).HasColumnName("IconClass");
            Property(t => t.HasChild).HasColumnName("HasChild");
            Property(t => t.SequenceNumber).HasColumnName("SequenceNumber");
            Property(t => t.DataType).HasColumnName("DataType");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
