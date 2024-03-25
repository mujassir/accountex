using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationJobContact : BaseEntity 
    {
        
        public int NotificationJobId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> UserId { get; set; }
        public bool IsAdminRole { get; set; }
        public bool IsAllUsers { get; set; }

    }
}
