using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationJobTrigger : BaseEntity
    {
        public byte TransactionTypeId { get; set; }
        public byte NotificationActionId { get; set; }
        public int ReferenceId { get; set; }
        public bool IsProcessed { get; set; }
        
    }
}
