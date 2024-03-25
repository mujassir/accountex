using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class PurchaseTotalItemMap : EntityTypeConfiguration<PurchaseTotalItem>
    {
        public PurchaseTotalItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.InvoiceNumber)
                .HasMaxLength(50);

            Property(t => t.VoucherNumber)
                .HasMaxLength(50);

            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("PurchaseTotalItems");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.PurchaseId).HasColumnName("PurchaseId");
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.Qty).HasColumnName("Qty");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
