using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class webpages_OAuthMembershipMap : EntityTypeConfiguration<webpages_OAuthMembership>
    {
        public webpages_OAuthMembershipMap()
        {
            // Primary Key
            HasKey(t => new { t.Provider, t.ProviderUserId });

            // Properties
            Property(t => t.Provider)
                .IsRequired()
                .HasMaxLength(30);

            Property(t => t.ProviderUserId)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            ToTable("webpages_OAuthMembership");
            Property(t => t.Provider).HasColumnName("Provider");
            Property(t => t.ProviderUserId).HasColumnName("ProviderUserId");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
