using System;

namespace AccountEx.Common
{
    public class  CustomerRecoveryEntry
    {
        
        public string CityName { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int VoucherNumber { get; set; }
        public byte TransactionType { get; set; }
        public string Comments { get; set; }
        public decimal Amount { get; set; }
    }
}
