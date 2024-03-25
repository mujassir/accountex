using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class vw_VouchersMap : EntityTypeConfiguration<vw_Vouchers>
    {
        public vw_VouchersMap()
        {
            // Primary Key
            HasKey(t => new { t.VoucherNumber, t.TransactionType, t.Date, t.Comments });

            // Properties
            Property(t => t.VoucherNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //Property(t => t.FromCode)
            //    .HasMaxLength(50);

            Property(t => t.AccountName)
                .HasMaxLength(250);

            //Property(t => t.ToCode)
            //    .HasMaxLength(50);

            //Property(t => t.To)
            //    .HasMaxLength(250);

            Property(t => t.Comments)
                .IsRequired();

            // Table & Column Mappings
            ToTable("vw_Vouchers");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.TransactionType).HasColumnName("TransactionType");
            Property(t => t.Date).HasColumnName("Date");
            //Property(t => t.FromCode).HasColumnName("FromCode");
            //Property(t => t.AccountName).HasColumnName("From");
            //Property(t => t.ToCode).HasColumnName("ToCode");
            //Property(t => t.To).HasColumnName("To");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.Comments).HasColumnName("Comments");
        }
    }
}
