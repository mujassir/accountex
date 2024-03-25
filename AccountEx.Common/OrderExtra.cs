using System;
namespace AccountEx.Common
{
    public class OrderExtra
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public int VoucherNumber { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public byte TransactionType { get; set; }
        public byte Status { get; set; }
        public Nullable<int> DcNo { get; set; }
        public Nullable<System.DateTime> DcDate { get; set; }
        
        public Nullable<byte> DCTransactionType { get; set; }
        public int? DCQuantity { get; set; }
        public int OrderTotalQuantity { get; set; }
        public int DcTotalQuantiy { get; set; }
        public decimal SaleTotalQuantiy { get; set; }
        public decimal NetTotal { get; set; }
        public string PartyPONumber { get; set; }





    }
}
