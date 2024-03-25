using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class UserProfileMap : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileMap()
        {
            // Primary Key
            HasKey(t => t.UserId);

            // Properties
            Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(56);

            // Table & Column Mappings
            ToTable("UserProfile");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.UserName).HasColumnName("UserName");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
