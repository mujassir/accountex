using System;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class Purchase : BaseEntity
    {
        public VoucherType TransactionType { get; set; }
        public int FiscalId { get; set; }
        public byte EnteryType { get; set; }
        public int AccountId { get; set; }
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string PurchaseType { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<double> StoreFare { get; set; }
        public Nullable<double> StoreLaboreCharges { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public string Vehicle { get; set; }
        public Nullable<int> Marka { get; set; }
        public string Challan { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsReceived { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}
