using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class RequisitionItem : BaseEntity
    {
        public int RequisitionId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal QtyDelivered { get; set; }
        public byte Status { get; set; }
    }
}
