using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public TransactionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("Transactions");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.TransactionType).HasColumnName("TransactionType");
            Property(t => t.EntryType).HasColumnName("EntryType");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Debit).HasColumnName("Debit");
            Property(t => t.Credit).HasColumnName("Credit");
            Property(t => t.Discount).HasColumnName("Discount");
            Property(t => t.Comments).HasColumnName("Comments");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.PartyAddress).HasColumnName("PartyAddress");
        }
    }
}
