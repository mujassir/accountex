using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.Transactions
{
  public partial  class PayablePayment:BaseEntity
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }   
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }    
        public string AccountCode { get; set; }
        public string AccountName { get; set; }   
        public VoucherType TransactionType { get; set; }      
        public Nullable<decimal> NetTotal { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> OldId { get; set; }
        public System.DateTime Date { get; set; }
        public int FiscalId { get; set; }
        public string BookNo { get; set; }
        public int VendorId { get; set; }
        public int VoucherId { get; set; }
    }
}
