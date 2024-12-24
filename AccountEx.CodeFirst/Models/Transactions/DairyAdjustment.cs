using System;

namespace AccountEx.CodeFirst.Models.Transactions
{
    public class DairyAdjustment : BaseEntity
    {
        public int VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Shift { get; set; }
        public int? ItemGroupId { get; set; }
        public string ItemGroupName { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Comment { get; set; }
        public decimal? Milk { get; set; }
        public decimal? ItemA { get; set; }
        public decimal? ItemB { get; set; }
        public decimal? ItemC { get; set; }
        public decimal? ItemD { get; set; }
        public decimal? Medicine { get; set; }
        public int? EntryType { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
