using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(500);

            Property(t => t.Manufacturer)
                .HasMaxLength(500);

            Property(t => t.Others)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Products");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Manufacturer).HasColumnName("Manufacturer");
            Property(t => t.Others).HasColumnName("Others");
            Property(t => t.PurchasePrice).HasColumnName("PurchasePrice");
            Property(t => t.SalePrice).HasColumnName("SalePrice");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.PackingPerCarton).HasColumnName("PackingPerCarton");
        }
    }
}
