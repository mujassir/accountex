using System;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class ProjectReceipt : BaseEntity
    {
        public int FiscalId { get; set; }
        public int ProjectId { get; set; }
        public int VoucherNumber { get; set; }
        public int BankReceiptId { get; set; }
        public VoucherType TransactionType { get; set; }
    }
}
