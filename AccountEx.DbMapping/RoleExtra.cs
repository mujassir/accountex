using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class RoleExtra
    {
        public RoleExtra()
        {
            Role = new Role();
            RoleAccess = new List<RoleAccess>();
            RoleActions = new List<RoleAction>();
        }
        public Role Role { get; set; }
        public List<RoleAccess> RoleAccess { get; set; }
        public List<RoleAction> RoleActions { get; set; }
    }
}
