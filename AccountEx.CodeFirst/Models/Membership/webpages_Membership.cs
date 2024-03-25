using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class webpages_Membership
    {
        public int UserId { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public Nullable<DateTime> LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public Nullable<DateTime> PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public Nullable<DateTime> PasswordVerificationTokenExpirationDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
