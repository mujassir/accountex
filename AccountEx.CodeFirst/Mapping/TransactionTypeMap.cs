using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class TransactionTypeMap : EntityTypeConfiguration<TransactionType>
    {
        public TransactionTypeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("TransactionTypes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
