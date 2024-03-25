using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ProjectReceiptMap : EntityTypeConfiguration<ProjectReceipt>
    {
        public ProjectReceiptMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            ToTable("ProjectReceipts");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ProjectId).HasColumnName("ProjectId");
            Property(t => t.VoucherNumber).HasColumnName("VoucherNumber");
            Property(t => t.BankReceiptId).HasColumnName("BankReceiptId");
        }
    }
}
