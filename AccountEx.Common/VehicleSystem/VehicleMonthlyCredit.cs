using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{
    public class VehicleMonthlyCredit
    {
        public int Id { get; set; }
        public System.DateTime SaleDate { get; set; }
        public string RegNo { get; set; }
        public string ChassisNo { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Deposit { get; set; }
        public decimal OtherBalance { get; set; }
        public decimal Balance { get; set; }
        public Nullable<System.DateTime> MonthlyPaymentDate { get; set; }
        public decimal PerMonthInstallment { get; set; }
        public Nullable<int> NoOfMonths { get; set; }
        public decimal TotalDue { get; set; }
        public decimal TotalReceivingWithoutDeposit { get; set; }
        public Nullable<decimal> TotalOverdueTillNow { get; set; }
        public decimal Penalty { get; set; }
        public Nullable<decimal> TotalBalanceAllRemaining { get; set; }
        public decimal MonthlyDue { get; set; }
        public decimal MonthlyReceiving { get; set; }
        public Nullable<decimal> MonthlyOverdue { get; set; }
        public string CustomerName { get; set; }
        public string LocalId { get; set; }
        public string ContactNumber { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactPersonNumber { get; set; }
        public string Address { get; set; }
        public string PoBoxNo { get; set; }
        public string PostalCode { get; set; }
        public string AgreementRemarks { get; set; }
    }
}
