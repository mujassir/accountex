
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public class Fiscal : BaseEntity
    {
        
        public string Name { get; set; }
        public string ShortName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsDefault { get; set; }
        public bool IsClosed { get; set; }
    }
}
