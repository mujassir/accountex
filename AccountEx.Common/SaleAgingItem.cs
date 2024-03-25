using System;

namespace AccountEx.Common
{
    public class SaleAgingItem
    {
        public int VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal DueAmount { get; set; }
        public decimal Balance { get; set; }
        public int Age { get; set; }
    }

   
}
