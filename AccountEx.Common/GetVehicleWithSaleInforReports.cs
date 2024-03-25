using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetVehicleWithSaleInforReports
    {
        public int VehicleId { get; set; }
        public int Id { get; set; }
        public Nullable<int> FileNo { get; set; }
        public Nullable<int> SaleId { get; set; }
        public string LogBookStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public System.DateTime LogBookDate { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public Nullable<System.DateTime> SoldDate { get; set; }
        public Nullable<System.DateTime> DeliverDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TrackerBalanceAmount { get; set; }
        public decimal InsurenceBalanceAmount { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal LogBookFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public decimal Received { get; set; }
        public decimal TradeInPrice { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public int AccountId { get; set; }
        public string Manufacturer { get; set; }
        public string BrandName { get; set; }
        public string CarType { get; set; }
        public string Model { get; set; }
        public string RegNo { get; set; }
        public string ChassisNo { get; set; }
        public string Color { get; set; }
        public string Year { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ShipName { get; set; }
        public string BLNumber { get; set; }

        
    }
}
