using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class webpages_RolesMap : EntityTypeConfiguration<webpages_Roles>
    {
        public webpages_RolesMap()
        {
            // Primary Key
            HasKey(t => t.RoleId);

            // Properties
            Property(t => t.RoleName)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            ToTable("webpages_Roles");
            Property(t => t.RoleId).HasColumnName("RoleId");
            Property(t => t.RoleName).HasColumnName("RoleName");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
