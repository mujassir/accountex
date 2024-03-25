using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.CodeFirst.Models.Views;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
namespace AccountEx.DbMapping
{
    public class BLStatusExtra : vw_Vehicles
    {
        public int BLId { get; set; }
        public string BLNo { get; set; }
        public string ShipperName { get; set; }
        public string ShipName { get; set; }
        public string SupplierName { get; set; }
        public string ConigneeName { get; set; }
    }

    public class BLSaveExtra : BL
    {
        public int BLId { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }
}
