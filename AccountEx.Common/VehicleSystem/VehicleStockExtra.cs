﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{
    public class VehicleStockExtra
    {

        public int Id { get; set; }
        public string FileNo { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string RegNo { get; set; }
        public string VehicleName { get; set; }
        public string BLNumber { get; set; }
        public string SupplierName { get; set; }
        public Nullable<System.DateTime> BLReceivedDate { get; set; }
        public string ShipName { get; set; }
        public string Consignee { get; set; }
        public string ClearingAgent { get; set; }
        public string Color { get; set; }
        public string Year { get; set; }
        public string CC { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal DutyTaxes { get; set; }
        public decimal SalePrice { get; set; }
        public decimal BLCharges { get; set; }
        public decimal MiscCharges { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public string CashHP { get; set; }
        public string Customer { get; set; }
        public string CustomerId { get; set; }
        public string ContactNumber { get; set; }
        public string AgreementRemarks { get; set; }
        public Nullable<byte> RecoveryStatus { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> Date { get; set; }
    }

}