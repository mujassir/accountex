using System;
namespace AccountEx.Common
{
    public class OrderByVoucherExtra
    {
        public int VoucherNumber { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public byte OrderType { get; set; }
        public byte Status { get; set; }
    }
}
