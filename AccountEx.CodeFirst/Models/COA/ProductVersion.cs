using System;

namespace AccountEx.CodeFirst.Models
{
    public class ProductVersion : BaseEntity
    {
        public string Version { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Description { get; set; }
        public string ChangeSet { get; set; }
        
    }
}
