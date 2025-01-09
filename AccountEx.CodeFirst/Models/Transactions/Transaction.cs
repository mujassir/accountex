using System;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class Transaction : BaseEntity
    {
        public int FiscalId { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public Nullable<int> MainEntityId { get; set; }

        public VoucherType TransactionType { get; set; }
        public byte EntryType { get; set; }
        public int AccountId { get; set; }
        public Nullable<int> ToAccountId { get; set; }
        public Nullable<int> ProductId { get; set; }

        
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string ChequeNumber { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PartyAddress { get; set; }
        public Nullable<int> BranchId { get; set; }
        public int AuthLocationId { get; set; }
        //public string ChequeNumber { get; set; }
    }
}
