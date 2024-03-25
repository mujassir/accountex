using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class webpages_UsersInRolesMap : EntityTypeConfiguration<webpages_UsersInRoles>
    {
        public webpages_UsersInRolesMap()
        {
            // Primary Key
            HasKey(t => new { t.UserId, t.RoleId });

            // Properties
            Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.RoleId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("webpages_UsersInRoles");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.RoleId).HasColumnName("RoleId");
            Property(t => t.CompanyId).HasColumnName("CompanyId");

            // Relationships
            HasRequired(t => t.UserProfile)
                .WithMany(t => t.webpages_UsersInRoles)
                .HasForeignKey(d => d.UserId);
            HasRequired(t => t.webpages_Roles)
                .WithMany(t => t.webpages_UsersInRoles)
                .HasForeignKey(d => d.RoleId);

        }
    }
}
