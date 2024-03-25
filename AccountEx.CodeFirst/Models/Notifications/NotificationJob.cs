using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationJob : BaseEntity 
    {
       
        public byte NotificationJobTypeId { get; set; }
        public string Name { get; set; }
        public bool IsNotification { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        public bool IsActive { get; set; }

     
    }
}
