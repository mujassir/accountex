using System;
using System.Collections.Generic;

namespace AccountEx.Common
{
    public class VehicleConfigExtra
    {

        public int AccountId { get; set; }
        public Nullable<bool> IsUniquePerVehicle { get; set; }
        public Nullable<bool> IsVehicleRequired { get; set; }
        public string AccountName { get; set; }
    }
    public class VehicleTradeIn
    {

        public int Id { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string RegNo { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
