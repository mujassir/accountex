using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class PurchaseMap : EntityTypeConfiguration<Purchase>
    {
        public PurchaseMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            Property(t => t.PurchaseType)
                .HasMaxLength(50);

            Property(t => t.Vehicle)
                .HasMaxLength(50);

            Property(t => t.Challan)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Purchases");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.TransactionType).HasColumnName("TransactionType");
            Property(t => t.EnteryType).HasColumnName("EnteryType");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.PurchaseType).HasColumnName("PurchaseType");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.Period).HasColumnName("Period");
            Property(t => t.StoreFare).HasColumnName("StoreFare");
            Property(t => t.StoreLaboreCharges).HasColumnName("StoreLaboreCharges");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Debit).HasColumnName("Debit");
            Property(t => t.Credit).HasColumnName("Credit");
            Property(t => t.Vehicle).HasColumnName("Vehicle");
            Property(t => t.Marka).HasColumnName("Marka");
            Property(t => t.Challan).HasColumnName("Challan");
            Property(t => t.Comments).HasColumnName("Comments");
            Property(t => t.IsReceived).HasColumnName("IsReceived");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
