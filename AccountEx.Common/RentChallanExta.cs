
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class PrintRentChallanExtra
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }
        public string ShopNo { get; set; }
        public string ShopCode { get; set; }
        public string Block { get; set; }
        public string Partner { get; set; }
        public string TenantName { get; set; }
        public string FileUrl { get; set; }
        public string Business { get; set; }
        public string Brand { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public byte AdjustmentType { get; set; }
        public Nullable<int> Unit { get; set; }
        public Nullable<int> PreviousReading { get; set; }
        public Nullable<int> CurrentReading { get; set; }
        public Nullable<decimal> ElectriciyUnitCharges { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public bool IsReceived { get; set; }
    }


    public class PrintOtherChallanExtra
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string ShopNo { get; set; }
        public string ShopCode { get; set; }
        public string Block { get; set; }
        public string Partner { get; set; }
        public string TenantName { get; set; }
        public string Business { get; set; }
        public string Brand { get; set; }
        public int NumberOfInstallment { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPerInstallment { get; set; }
        public int TotalInstallment { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
    }
    public class PrintMiscChallanExtra
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string ShopNo { get; set; }
        public string ShopCode { get; set; }
        public string Block { get; set; }
        public string Partner { get; set; }
        public string TenantName { get; set; }
        public string Business { get; set; }
        public string Brand { get; set; }
        public decimal NetAmount { get; set; }
        public int NumberOfInstallment { get; set; }
        public string FinedPerson { get; set; }
        public string Remarks { get; set; }
        public Nullable<DateTime> DueDate { get; set; }

    }

    public class PaidRentChallanExta
    {

        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
        public decimal TotalAmount { get; set; }

    }
    public class ReceiveRentChallanExtra
    {

        public int Id { get; set; }
        public Nullable<DateTime> ReceiveDate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal LateSurCharge { get; set; }
        public byte TransactionType { get; set; }



    }
    public class AllChallanExtra
    {

        public int VoucherNumber { get; set; }
        public int Id { get; set; }
        public bool IsAuto { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public bool IsReceived { get; set; }
        public int RcvNo { get; set; }
        public string Duration { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }

    }
    public class RentChallanExta
    {
        public PaidRentChallanExta PaidRentChallan { get; set; }
        public RentDetailExtra RentDetail { get; set; }
        public List<AllChallanExtra> AllChallans { get; set; }
    }
    public class ElectricityChallanExtra
    {
        public PaidRentChallanExta PaidRentChallan { get; set; }
        public ElectricityChallan ElectricityChallan { get; set; }
        public List<AllChallanExtra> AllChallans { get; set; }
    }

    public class ElectricityChallan
    {
        public int Id { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
    }

}
