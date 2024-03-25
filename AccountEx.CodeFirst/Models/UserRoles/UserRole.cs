using System;

namespace AccountEx.CodeFirst.Models
{
     [Serializable]
    public partial class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
