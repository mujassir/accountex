using System.Collections.Generic;
namespace AccountEx.CodeFirst.Models
{
    public partial class Role : BaseEntity
    {
        public Role()
        {
            RoleAccess = new HashSet<RoleAccess>();
            RoleActions = new HashSet<RoleAction>();
        }
        public string Name { get; set; }
        public int? DashBoardId { get; set; }

        public ICollection<RoleAccess> RoleAccess { get; set; }
        public ICollection<RoleAction> RoleActions { get; set; }
    }
}
