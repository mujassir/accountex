using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class LeadExpectedItem :BaseEntity
    {
        public int LeadId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
       
    }
}
