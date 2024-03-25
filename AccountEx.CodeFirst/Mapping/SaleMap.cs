using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class SaleMap : EntityTypeConfiguration<Sale>
    {
        public SaleMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.AccountTitle)
                .HasMaxLength(500);

            //this.Property(t => t.Comments)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Sales");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.ReceivingVoucher).HasColumnName("ReceivingVoucher");
          
            Property(t => t.Comments).HasColumnName("Comments");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
