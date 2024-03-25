using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class StorePurchaseTotalItem : BaseEntity
    {
        public Nullable<int> PurchaseId { get; set; }
        public string InvoiceNumber { get; set; }
        public string VoucherNumber { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string AccountTitle { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<DateTime> Date { get; set; }
    }
}
