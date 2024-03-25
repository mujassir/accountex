using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class StoreTransaction : BaseEntity
    {
        public byte TransactionType { get; set; }
        public byte EnteryType { get; set; }
        public int AccountId { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public string Vehicle { get; set; }
        public Nullable<int> Marka { get; set; }
        public string PurchaseType { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<bool> IsReceived { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<double> StoreFare { get; set; }
        public Nullable<double> LaboreCharges { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}
