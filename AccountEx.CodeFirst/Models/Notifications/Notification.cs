using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class Notification : BaseEntity 
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        
        public Nullable<DateTime> ModifiedAt { get; set; }
        public Nullable<int> ModifiedBy { get; set; }

    }
}
