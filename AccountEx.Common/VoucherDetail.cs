using System;

namespace AccountEx.Common
{
    public partial class VoucherDetail
    {
        public DateTime Date { get; set; }
        public VoucherType VoucherType { get; set; }
        public int VoucherNumber { get; set; }
        public string Remarks { get; set; }
        public decimal Cash { get; set; }
        public decimal Bank { get; set; }
        public decimal Rent { get; set; }
        public decimal UtitlityCharges { get; set; }
        public decimal Electricity { get; set; }
        public decimal PossessionCharges { get; set; }
        public decimal TfrFee { get; set; }
        public decimal SecurityMoney { get; set; }
        public decimal PromoActivity { get; set; }
        public decimal CarParking { get; set; }
        public decimal BankProfit { get; set; }
        public decimal Surcharge { get; set; }
        public decimal Misc { get; set; }
        public decimal Crs { get; set; }
        public decimal Drs { get; set; }
        public decimal Misc4 { get; set; }
        public decimal Misc5 { get; set; }

    }
}
