using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class WPItem : BaseEntity
    {
        public int WorkInProgressId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public byte EntryType { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalWeight { get; set; }
        public int OrderNo { get; set; }
        public int RequisitionNo { get; set; }
        public int GINPNo { get; set; }
        public decimal Width { get; set; }
        public decimal Meters { get; set; }
        public decimal Rolls { get; set; }
        public decimal TotalRolls { get; set; }
        public decimal TotalMeters { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string Comments { get; set; }
    }
}
