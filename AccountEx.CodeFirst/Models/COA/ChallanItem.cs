using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.COA
{
    public class ChallanItem : BaseEntity
    {
        public int ChallanId { get; set; }
        public Nullable<int> RentAgreementId { get; set; }
        public Nullable<int> ElectricityUnitItemId { get; set; }
        public Nullable<int> ElectricityUnitId { get; set; }
        public int VoucherNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public Nullable<int> ShopId { get; set; }
        public string ShopNo { get; set; }
        public int TenantAccountId { get; set; }
        public string TenantAccountName { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
        public decimal LateSurCharge { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public int Unit { get; set; }
        public string TenantCode { get; set; }
        public string Business { get; set; }
        public string Brand { get; set; }
        public bool IsReceived { get; set; }
        public bool Status { get; set; }
        public VoucherType TransactionType { get; set; }
        public byte EntryType { get; set; }

        public string FinedPerson { get; set; }
        public string Remarks { get; set; }

    }
}
