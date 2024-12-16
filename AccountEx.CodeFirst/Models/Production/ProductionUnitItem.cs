using AccountEx.Common;
using System;
namespace AccountEx.CodeFirst.Models.Production
{
    public class ProductionUnitItem : BaseEntity
    {
        public int ProductionUnitId { get; set; }
        public ProductionUnitItemType Type { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int VoucherNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvailQty { get; set; }
        public decimal Balance { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal? ConsumeQty { get; set; }
        public decimal? WasteQty { get; set; }
        public decimal? RemainingQty { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}
