using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AttributeMap : EntityTypeConfiguration<Attribute>
    {
        public AttributeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(100);

            Property(t => t.Label)
                .HasMaxLength(500);

            Property(t => t.TypeName)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Attributes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountTypeId).HasColumnName("AccountTypeId");
            Property(t => t.TypeId).HasColumnName("TypeId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Label).HasColumnName("Label");
            Property(t => t.TypeName).HasColumnName("TypeName");
            Property(t => t.SequenceNumber).HasColumnName("SequenceNumber");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
