using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.RentalAgreement
{
    public partial class GetRentRecoveryDetail
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal SurCharge { get; set; }
        public decimal LateSurcharge { get; set; }
        public decimal ReceivedSurChargeArrears { get; set; }
        public decimal ReceivedCurrentSurCharge { get; set; }
        public decimal BalanceSurCharge { get; set; }

        public decimal TotalRent { get; set; }
        public decimal TotalUC { get; set; }
        public decimal TotalElectricity { get; set; }
        public decimal ReceivedRentArrears { get; set; }
        public decimal ReceivedUCPercentArears { get; set; }
        public decimal ReceivedElectricityArrears { get; set; }
        public decimal ReceivedMonthlyRent { get; set; }
        public decimal ReceivedUCPercent { get; set; }
        public decimal ReceivedElectricityCharges { get; set; }
        public decimal ReceivedSurCharge { get; set; }
        public decimal BalanceRent { get; set; }
        public decimal BalanceUC { get; set; }
        public decimal BalanceElectricity { get; set; }
    }
}
