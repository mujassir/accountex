﻿using System;
namespace AccountEx.Common
{
    public class ChallanInfoExtra
    {
        public int ShopId { get; set; }
        public string ShopNo { get; set; }

        public string TenantAccountName { get; set; }
        public int TenantAccountId { get; set; }
        public string Business { get; set; }
        public int RentAgreementId { get; set; }
        public string TenantCode { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public int ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public int ElectricityUnitId { get; set; }
        public int ElectricityUnitItemId { get; set; }
    }
}
