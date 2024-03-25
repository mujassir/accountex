using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AttributeTypeMap : EntityTypeConfiguration<AttributeType>
    {
        public AttributeTypeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(100);

            Property(t => t.ControlType)
                .HasMaxLength(50);

            Property(t => t.CssClass)
                .HasMaxLength(100);

            Property(t => t.SizeId)
                .HasMaxLength(50);

            Property(t => t.SizeName)
                .HasMaxLength(50);

            Property(t => t.Data)
                .HasMaxLength(1000);

            // Table & Column Mappings
            ToTable("AttributeTypes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ControlType).HasColumnName("ControlType");
            Property(t => t.CssClass).HasColumnName("CssClass");
            Property(t => t.SizeId).HasColumnName("SizeId");
            Property(t => t.SizeName).HasColumnName("SizeName");
            Property(t => t.Data).HasColumnName("Data");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
