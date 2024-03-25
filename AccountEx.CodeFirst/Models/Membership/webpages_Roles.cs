using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class webpages_Roles
    {
        public webpages_Roles()
        {
            webpages_UsersInRoles = new List<webpages_UsersInRoles>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public virtual ICollection<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
    }
}
