using System;

namespace AccountEx.CodeFirst.Models
{


    public partial class AccountDetail : BaseEntity
    {


        public int AccountId { get; set; }
        public int ParentId { get; set; }
        public int AccountDetailFormId { get; set; }
        public Nullable<int> SalemanId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> OrderTakerId { get; set; }
        public Nullable<int> TerritoryManagerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string ContactNumber { get; set; }
        public string FaxNumber { get; set; }
        public string HomePhone { get; set; }
        public string PostalCode { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string GST { get; set; }
        public string NTN { get; set; }
        public string Others { get; set; }
        public string CNIC { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public string BloodGroup { get; set; }
        public string PermanentAddress { get; set; }
        public string Qualification { get; set; }
        public string Designation { get; set; }
        public int DesignationId { get; set; }
        public int Department { get; set; }
        public string PictureUrl { get; set; }
        public string Reference { get; set; }
        public string PassportNumber { get; set; }
        public string DeploymentStatus { get; set; }
        public Nullable<DateTime> DOB { get; set; }
        public Nullable<DateTime> DOJ { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public decimal Weight { get; set; }
        public decimal Freight { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> PackingPerCarton { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string IBN { get; set; }
        public string SwiftCode { get; set; }
        public string Location { get; set; }
        public string AssetDetail { get; set; }
        public string Value { get; set; }
        public string AssetType { get; set; }
        public Nullable<decimal> AnnualAllowed { get; set; }
        public Nullable<decimal> SickAllowed { get; set; }
        public Nullable<decimal> CasualAllowed { get; set; }
        public Nullable<decimal> CompensateryAllowed { get; set; }
        public string SalemanName { get; set; }
        public string SalemanCell { get; set; }
        public string SalemanAddress { get; set; }
        public string ContactPersonNumber { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public string GroupName { get; set; }
        public int Unit { get; set; }
        public string BNo { get; set; }
        public Nullable<DateTime> PurchaseDate { get; set; }
        public Nullable<DateTime> MFDDate { get; set; }
        public Nullable<DateTime> ExpDate { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> PackQuantity { get; set; }
        public Nullable<int> InPackQuantity { get; set; }
        public Nullable<int> MedicineQuantity { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> MRP { get; set; }
        public string StoragePlace { get; set; }
        public string PurchaseFrom { get; set; }
        public string Remarks { get; set; }
        public string Symptoms { get; set; }
        public Nullable<bool> IsUnit { get; set; }
        public Nullable<bool> IsStrip { get; set; }
        public Nullable<bool> IsPack { get; set; }
        public Nullable<bool> IsCotton { get; set; }
        public string Generic { get; set; }
        public string Brand { get; set; }
        public string PackagingType { get; set; }
        public Nullable<int> GenericId { get; set; }


        public Nullable<int> PPCQuantity { get; set; }
        public Nullable<decimal> PPCSaleRate { get; set; }
        public Nullable<decimal> PPCPurchaseRate { get; set; }
        public Nullable<int> SPPQuantity { get; set; }
        public Nullable<decimal> SPPSaleRate { get; set; }
        public Nullable<decimal> SPPPurchaseRate { get; set; }
        public decimal CommissionPercent { get; set; }


        public Nullable<int> OLDID { get; set; }
        public string RegistrationNo { get; set; }
        public string RegistrationName { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }
        public string Area { get; set; }
        public string PoBoxNo { get; set; }
        public string Route { get; set; }
        public string LocalId { get; set; }
        public string KhasraNo { get; set; }
        public string PriceType { get; set; }
        public string BarCode { get; set; }
        public string Company { get; set; }
        public string MadeIn { get; set; }
        public string SrNo { get; set; }
        public string Readings { get; set; }
        public string ArticleNo { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitType { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public string VendorCode { get; set; }
        public Nullable<int> ParentVendorAccountId { get; set; }
        public bool OwnVendorCode { get; set; }
        public string BrandName { get; set; }
        public string Business { get; set; }
        public string Type { get; set; }
        public string CompanyCode { get; set; }
        public decimal SaleDiscount { get; set; }
        public decimal PurchaseDiscount { get; set; }
        public string MedicineNote { get; set; }
        public int BankId { get; set; }
        public Nullable<int> CollectionAccountId { get; set; }

        public decimal CreditLimitValue { get; set; }
        public string CreditLimit { get; set; }
        public Nullable<int> BrandId { get; set; }
        public int NumberOfPartners { get; set; }

        public string HSCode { get; set; }
        public bool ShowBarcodeBox { get; set; }
        public string GSTJurisdiction { get; set; }

        public decimal MainUnitSaleRale { get; set; }
        public decimal MainUnitPurchaseRale { get; set; }
        public Nullable<bool> IsUniquePerVehicle { get; set; }
        public Nullable<bool> IsVehicleRequired { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public bool IsForex { get; set; }
        public int AuthLocationId { get; set; }
        public decimal? MaxAmountThreshold { get; set; }

    }
}
