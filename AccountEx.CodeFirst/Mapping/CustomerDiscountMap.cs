using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class CustomerDiscountMap : EntityTypeConfiguration<CustomerDiscount>
    {
        public CustomerDiscountMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.CustomerTitle)
                .HasMaxLength(100);

            Property(t => t.ProductTitle)
                .HasMaxLength(500);

            Property(t => t.ProductCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("CustomerDiscounts");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CustomerId).HasColumnName("CustomerId");
            Property(t => t.CustomerTitle).HasColumnName("CustomerTitle");
            Property(t => t.ProductId).HasColumnName("ProductId");
            Property(t => t.ProductTitle).HasColumnName("ProductTitle");
            Property(t => t.COAProductId).HasColumnName("COAProductId");
            Property(t => t.ProductCode).HasColumnName("ProductCode");
            Property(t => t.Discount).HasColumnName("Discount");
        }
    }
}
