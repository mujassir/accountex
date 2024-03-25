using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.RentalAgreement
{
    public class GetRecoveryOfPossessionCharge
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }
        public decimal DueArrears { get; set; }
        public decimal DueCurrent { get; set; }
        public decimal TotalDue { get; set; }
        public decimal ReceivedArrears { get; set; }
        public decimal ReceivedCurrent { get; set; }
        public decimal TotalReceived { get; set; }
        public int ReceivedInstallments { get; set; }
        public int TotalInstallments { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
    public class GetRecoveryOfPossessionCharge1
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }
        public string Block { get; set; }
        public decimal Due { get; set; }
        public decimal Received { get; set; }

    }
}
