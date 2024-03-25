using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class vw_TransactionMap : EntityTypeConfiguration<vw_Transaction>
    {
        public vw_TransactionMap()
        {
            // Primary Key
            HasKey(t => new { t.Id, t.TransactionType, t.EntryType, t.AccountId, t.InvoiceNumber, t.VoucherNumber, t.Quantity, t.Price, t.Debit, t.Credit, t.Date, t.CreatedDate, t.IsDeleted });

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AccountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            Property(t => t.InvoiceNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.VoucherNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Quantity)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Price)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Debit)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Credit)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Account)
                .HasMaxLength(250);

            // Table & Column Mappings
            ToTable("vw_Transaction");
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
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.PartyAddress).HasColumnName("PartyAddress");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Account).HasColumnName("Account");
        }
    }
}
