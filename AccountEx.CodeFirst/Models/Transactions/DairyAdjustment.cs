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
        public int? DebitItem1Code { get; set; }
        public decimal? DebitItem1 { get; set; }
        public int? DebitItem2Code { get; set; }
        public decimal? DebitItem2 { get; set; }
        public int? CreditItem1Code { get; set; }
        public decimal? CreditItem1 { get; set; }
        public int? CreditItem2Code { get; set; }
        public decimal? CreditItem2 { get; set; }
        public int? CreditItem3Code { get; set; }
        public decimal? CreditItem3 { get; set; }
        public int? CreditItem4Code { get; set; }
        public decimal? CreditItem4 { get; set; }
        public int? CreditItem5Code { get; set; }
        public decimal? CreditItem5 { get; set; }
        public int? CreditItem6Code { get; set; }
        public decimal? CreditItem6 { get; set; }
        public int? CreditItem7Code { get; set; }
        public decimal? CreditItem7 { get; set; }
        public int? CreditItem8Code { get; set; }
        public decimal? CreditItem8 { get; set; }
        public int? EntryType { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
