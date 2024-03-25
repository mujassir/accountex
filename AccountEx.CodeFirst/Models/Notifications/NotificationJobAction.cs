using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationJobAction : BaseEntity 
    {
       
        public int NotificationJobId { get; set; }
        public byte TransactionTypeId { get; set; }
        public byte NotificationActionId { get; set; }

     
    }
}
