using System;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class vw_Transaction
    {
        public int Id { get; set; }
        public VoucherType TransactionType { get; set; }
        public byte EntryType { get; set; }
        public int AccountId { get; set; }
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string PartyAddress { get; set; }
        public string Code { get; set; }
        public string Account { get; set; }
    }
}
