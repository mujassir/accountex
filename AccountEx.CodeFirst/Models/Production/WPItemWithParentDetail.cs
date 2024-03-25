using AccountEx.Common;
using System;
namespace AccountEx.CodeFirst.Models.Production
{
    public class WPItemWithParentDetail
    {
        // WorkInProgress properties
        public int OrderNo { get; set; }
        public VoucherType VoucherType { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }

        // WPItem properties
        public int Id { get; set; } // Primary key of WPItem
        public int WorkInProgressId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalWeight { get; set; }
        public Nullable<int> RequisitionNo { get; set; }
        public Nullable<int> GINPNo { get; set; }
        public decimal Width { get; set; }
        public decimal Meters { get; set; }
        public decimal Rolls { get; set; }
        public decimal TotalRolls { get; set; }
        public decimal TotalMeters { get; set; }
        public string Comments { get; set; }

    }
}
