using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class vw_Vouchers1
    {
        public int VoucherNumber { get; set; }
        public byte TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string FromCode { get; set; }
        public string From { get; set; }
        public string ToCode { get; set; }
        public string To { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Comments { get; set; }
    }
}
