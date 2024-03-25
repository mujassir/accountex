using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class UserExtra
    {
        public UserExtra()
        {
            User = new User();
            RoleAccess = new List<RoleAccess>();
            RoleActions = new List<RoleAction>();
        }
        public User User { get; set; }
        public List<RoleAccess> RoleAccess { get; set; }
        public List<RoleAction> RoleActions { get; set; }
    }
}
