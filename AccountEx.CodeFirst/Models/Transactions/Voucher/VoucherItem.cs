using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class VoucherItem : BaseEntity
    {
        public int VoucherId { get; set; }
        public int VoucherNumber { get; set; }
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public string ChequeNumber { get; set; }
        public Nullable<DateTime> Dated { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<int> BLId { get; set; }
        public Nullable<int> BLChargeId { get; set; }
        public Nullable<int> CostCenterId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }


    }
}
