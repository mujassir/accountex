using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class AssetMap : EntityTypeConfiguration<Asset>
    {
        public AssetMap()
        {
            // Primary Key
            HasKey(t => new { t.Id, t.AccountId });

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AccountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(50);

            Property(t => t.Location)
                .HasMaxLength(500);

            Property(t => t.AssetDetail)
                .HasMaxLength(500);

            Property(t => t.Value)
                .HasMaxLength(50);

            Property(t => t.Others)
                .HasMaxLength(500);

            Property(t => t.AssetType)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Assets");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Location).HasColumnName("Location");
            Property(t => t.AssetDetail).HasColumnName("AssetDetail");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.Others).HasColumnName("Others");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
            Property(t => t.AssetType).HasColumnName("AssetType");
        }
    }
}
