using System;
namespace AccountEx.Common
{

    public class COAEx
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string AccountCode { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte Level { get; set; }
        public int AccountTypeId { get; set; }




    }
    public class DiscountTradeEx
    {
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> COAProductId { get; set; }
        public Nullable<int> Discount { get; set; }



    }
    public class IRISTransEx
    {
        public int AccountId { get; set; }
        public decimal Rate { get; set; }
        public string BarCode { get; set; }
    }


    public class TransSaleEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Rate { get; set; }
    }

    public class UltraTechTransSaleEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Rate { get; set; }
        public string HSCode { get; set; }
    }

    public class AdjustmentEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Rate { get; set; }
    }
    public class NoumtexSaleEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
    }


    public class UltratechDC
    {
        public int AccountId { get; set; }
        public string ArticleNo { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
    }

    public class NoumtexDCEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
    }
    public class TradeSaleEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SaleDiscount { get; set; }
        public string BarCode { get; set; }
    }
    public class TradePurchaseEx
    {
        public int AccountId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal PurchaseDiscount { get; set; }
        public string BarCode { get; set; }
    }
    public class TransWeightSaleEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string PriceType { get; set; }


    }
    public class TransWeightPurchaseEx
    {
        public int AccountId { get; set; }
        public decimal SalePrice { get; set; }
        public string BarCode { get; set; }
        public string PriceType { get; set; }


    }
    public class TransPurchaseEx
    {
        public int AccountId { get; set; }
        public decimal PurchasePrice { get; set; }
        public string BarCode { get; set; }
    }
    public class NoumtexPurchaseEx
    {
        public int AccountId { get; set; }
        public decimal PurchasePrice { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public decimal Weight { get; set; }
    }
    public class DiscountEx
    {
        public int CustomerId { get; set; }
        public int COAProductId { get; set; }
        public decimal Discount { get; set; }

    }

    public class TransporterEx
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class UltratechPurchaseEx
    {
        public int AccountId { get; set; }
        public decimal PurchasePrice { get; set; }
        public string BarCode { get; set; }
        //        public int Unit { get; set; }
        public string UnitType { get; set; }

    }
    public class ShipperEx
    {
        public int AccountId { get; set; }
        public decimal Name { get; set; }
        public string Address { get; set; }
    }

    public class OrderDcEx
    {
        public int AccountId { get; set; }
        public string BarCode { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string ArticleNo { get; set; }
        public int Rate { get; set; }
        public string UnitType { get; set; }


    }

    public class OrderEx
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
    public class FlourSaleEx
    {
        public int AccountId { get; set; }
        public string BarCode { get; set; }
        public decimal Weight { get; set; }
        public decimal Freight { get; set; }
        public decimal SalePrice { get; set; }
    }
    public class RequisitionEx
    {
        public int AccountId { get; set; }
        public string BarCode { get; set; }
        public string UnitType { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string ArticleNo { get; set; }
        public int Rate { get; set; }
    }
    public class JobCardEx
    {
        public int AccountId { get; set; }
        public decimal Rate { get; set; }
        public string BarCode { get; set; }
        public string SrNo { get; set; }
    }
    public class ServiceOrderEx
    {
        public int AccountId { get; set; }
        public decimal Rate { get; set; }
        public string BarCode { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string SrNo { get; set; }
    }
    public class EquipmentEx
    {
        public int AccountId { get; set; }
        public string SrNo { get; set; }
    }
    public class CustomerVendorCodesEx
    {
        public int AccountId { get; set; }
        public string VendorCode { get; set; }
    }
    public class PharmacySaleEx
    {
        public int AccountId { get; set; }
        public decimal Rate { get; set; }
        public string BarCode { get; set; }
        public Nullable<bool> IsUnit { get; set; }
        public Nullable<bool> IsStrip { get; set; }
        public Nullable<bool> IsPack { get; set; }
        public Nullable<bool> IsCotton { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> PPCQuantity { get; set; }
        public Nullable<int> SPPQuantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Location { get; set; }
    }
    public class TenantEx
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string Brand { get; set; }
        public string ContactNumber { get; set; }
        public string CNIC { get; set; }
        public int NumberOfPartners { get; set; }
    }

}
