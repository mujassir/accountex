using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class webpages_UsersInRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual webpages_Roles webpages_Roles { get; set; }
    }
}
