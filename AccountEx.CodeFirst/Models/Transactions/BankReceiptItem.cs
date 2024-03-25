using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Transactions
{
    public class BankReceiptItem : BaseEntity
    {
        public int ChallanItemId { get; set; }
        public int BankReceiptId { get; set; }
        public int TenantAccountId { get; set; }
        public decimal MonthlyRent { get; set; }
        public string ShopNo { get; set; }
        public string TenantAccountName { get; set; }
        public string Description { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercent { get; set; }
        public decimal UCPercentArears { get; set; }
        public int ArrearUnit { get; set; }
        public int Unit { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal Amount { get; set; }

    }
}
