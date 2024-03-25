using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class SaleItemMap : EntityTypeConfiguration<SaleItem>
    {
        public SaleItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

         

            // Table & Column Mappings
            ToTable("SaleItems");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SaleId).HasColumnName("SaleId");
          
            Property(t => t.ItemId).HasColumnName("ItemId");
        
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.Quantity).HasColumnName("Quantity");
         
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");

          

        }
    }
}
