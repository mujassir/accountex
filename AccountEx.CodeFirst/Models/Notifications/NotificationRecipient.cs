using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationRecipient : BaseEntity 
    {
        public int NotificationId { get; set; }
        public int RecipientUserId { get; set; }
        public bool IsNotificationRead { get; set; }
        public bool IsInboxRead { get; set; }

    }
}
