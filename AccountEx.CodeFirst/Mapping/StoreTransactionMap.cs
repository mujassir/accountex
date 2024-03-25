using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class StoreTransactionMap : EntityTypeConfiguration<StoreTransaction>
    {
        public StoreTransactionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Vehicle)
                .HasMaxLength(50);

            Property(t => t.PurchaseType)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("StoreTransactions");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.TransactionType).HasColumnName("TransactionType");
            Property(t => t.EnteryType).HasColumnName("EnteryType");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Debit).HasColumnName("Debit");
            Property(t => t.Credit).HasColumnName("Credit");
            Property(t => t.Vehicle).HasColumnName("Vehicle");
            Property(t => t.Marka).HasColumnName("Marka");
            Property(t => t.PurchaseType).HasColumnName("PurchaseType");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.IsReceived).HasColumnName("IsReceived");
            Property(t => t.Period).HasColumnName("Period");
            Property(t => t.StoreFare).HasColumnName("StoreFare");
            Property(t => t.LaboreCharges).HasColumnName("LaboreCharges");
            Property(t => t.Total).HasColumnName("Total");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
