using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class webpages_OAuthMembership
    {
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public int UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
