using System;

namespace AccountEx.CodeFirst.Models.Transactions
{
    public class DairyTransaction : BaseEntity
    {
        public int VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public string Shift { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Comment { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalQty { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? EntryType { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
