using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.RentalAgreement
{
    public class GetOverallRecoveryReport
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }

        public decimal RentArrears { get; set; }

        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal TotalRent { get; set; }
        public decimal TotalUC { get; set; }
        public decimal TotalElectricity { get; set; }
        public decimal ReceivedRentArrears { get; set; }
        public decimal ReceivedElectricityArrears { get; set; }


        public decimal ReceivedUCPercentArears { get; set; }
        public decimal ReceivedMonthlyRent { get; set; }
        public decimal ReceivedUCPercent { get; set; }
        public decimal ReceivedElectricityCharges { get; set; }
        public decimal BalanceRent { get; set; }
        public decimal BalanceUC { get; set; }
        public decimal BalanceElectricity { get; set; }
    }
}
