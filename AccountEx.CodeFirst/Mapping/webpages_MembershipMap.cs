using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class webpages_MembershipMap : EntityTypeConfiguration<webpages_Membership>
    {
        public webpages_MembershipMap()
        {
            // Primary Key
            HasKey(t => t.UserId);

            // Properties
            Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ConfirmationToken)
                .HasMaxLength(128);

            Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.PasswordVerificationToken)
                .HasMaxLength(128);

            // Table & Column Mappings
            ToTable("webpages_Membership");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.ConfirmationToken).HasColumnName("ConfirmationToken");
            Property(t => t.IsConfirmed).HasColumnName("IsConfirmed");
            Property(t => t.LastPasswordFailureDate).HasColumnName("LastPasswordFailureDate");
            Property(t => t.PasswordFailuresSinceLastSuccess).HasColumnName("PasswordFailuresSinceLastSuccess");
            Property(t => t.Password).HasColumnName("Password");
            Property(t => t.PasswordChangedDate).HasColumnName("PasswordChangedDate");
            Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            Property(t => t.PasswordVerificationToken).HasColumnName("PasswordVerificationToken");
            Property(t => t.PasswordVerificationTokenExpirationDate).HasColumnName("PasswordVerificationTokenExpirationDate");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
