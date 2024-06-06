using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class SaleItem : BaseEntity
    {
        public int SaleId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public int SalesmanId { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public decimal NTQty { get; set; }
        public Nullable<decimal> MainUnitQuantity { get; set; }
        public string Unit { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal GSTPercent { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal BonusMainUnitQuantity { get; set; }
        public decimal BonusQuantity { get; set; }
        public string BonusUnit { get; set; }

        public decimal Rate { get; set; }
        public decimal Weight { get; set; }
        public decimal Freight { get; set; }
        public decimal NetAmount { get; set; }
        public byte TransactionType { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string PriceType { get; set; }
        public Nullable<int> Godown { get; set; }
        public Nullable<int> Size { get; set; }
        public string Comment { get; set; }
        public int SaleTaxNo { get; set; }
        public decimal AIT { get; set; }
        public decimal CD { get; set; }
        public decimal RD { get; set; }
        public decimal SED { get; set; }
        public decimal FWD { get; set; }
        public decimal Others { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ComissionPercent { get; set; }
        public decimal ComissionAmount { get; set; }
        public decimal DiscountPerItem { get; set; }
        public decimal PromotionPercent { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal Width { get; set; }
        public decimal Meters { get; set; }
        public decimal Rolls { get; set; }
        public decimal TotalMeters { get; set; }
        public decimal StandardWeight { get; set; }
        public string UnitType { get; set; }
   
        public string BatchNo { get; set; }

        public decimal WHTAmount { get; set; }
        public decimal WHTPercent { get; set; }


        public Nullable<decimal> MaximumRetailPrice { get; set; }
        public Nullable<decimal> TradePrice { get; set; }
        public string PKG { get; set; }
        public Nullable<decimal> PackingPerCarton { get; set; }
        public Nullable<int> DeliveryChallanDetailId { get; set; }
        public Nullable<int> OrderDetailId { get; set; }
        public Nullable<int> Morning { get; set; }
        public Nullable<int> Afternoon { get; set; }
        public Nullable<int> Evening { get; set; }
        public Nullable<int> Days { get; set; }

        public Nullable<decimal> AvQty { get; set; }

        public Nullable<decimal> AvRate { get; set; }

        public Nullable<decimal> AvAmount { get; set; }
    }
}
