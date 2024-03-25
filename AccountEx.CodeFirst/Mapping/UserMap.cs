using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Username)
                .HasMaxLength(50);

            Property(t => t.FirstName)
                .HasMaxLength(50);

            Property(t => t.LastName)
                .HasMaxLength(50);

            Property(t => t.Email)
                .HasMaxLength(50);

            Property(t => t.Hash)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Users");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Username).HasColumnName("Username");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.Hash).HasColumnName("Hash");
            Property(t => t.LastLogin).HasColumnName("LastLogin");
            Property(t => t.IsLive).HasColumnName("IsLive");
            Property(t => t.IsSystemUser).HasColumnName("IsSystemUser");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
