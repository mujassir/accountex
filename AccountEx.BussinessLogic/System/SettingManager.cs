using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.CodeFirst.Models;

namespace AccountEx.BussinessLogic
{
    public static class SettingManager
    {
        #region Properties
        public static string Assets
        {
            get { return Get("COA.Assets"); }
        }
        public static string ApplicationPrintTitle
        {
            get { return Get("Application.PrintTitle"); }
        }
        public static string Customers
        {
            get { return Get("COA.Customers"); }
        }
        public static string Suppliers
        {
            get { return Get("COA.Suppliers"); }
        }
        public static string Employees
        {
            get { return Get("COA.Employees"); }
        }

        public static string ApplicationTitle
        {
            get { return Get("Application.Title"); }
        }
        public static string ApplicationLogo
        {
            get { return Get("Application.LogoUrl"); }
        }
        public static string LoginLogo
        {
            get { return Get("Application.LoginLogoUrl"); }
        }
        public static string BillLogo
        {
            get { return Get("Application.BillLogoUrl"); }
        }
        public static string ReportHeader
        {
            get { return Get("Report.Header"); }
        }
        public static string MenuType
        {
            get { return Get("Application.MenuType"); }
        }
        public static bool BarCodeEnabled
        {
            get { return Get<bool>("Application.BarCodeEnabled", false); }
        }
        public static bool ReportExportEnabled
        {
            get { return Get<bool>("Application.ReportExportEnabled", false); }
        }
        public static int BarcodeHeight
        {
            get { return Get<int>("Application.BarcodeHeight", 50); }
        }
        public static int BarcodeWidth
        {
            get { return Get<int>("Application.BarcodeWidth", 200); }
        }
        public static string BarcodeType
        {
            get { return Get<string>("Application.BarcodeType", "CODE39"); }
        }
        public static bool IsAppLogoVisible
        {
            get { return Get<bool>("Application.ShowHideAppLogo", false); }
        }
        public static bool AllowDC
        {
            get { return Get<bool>("DeliveryChallan.AllowDC", false); }
        }
        public static decimal Gst
        {
            get { return Get<decimal>("Taxes.GST", 17.0); }

        }

        public static decimal DiscountPercentage
        {
            get { return Get<decimal>("Taxes.Discount", 0); }

        }
        public static decimal ElectricityPerUnitCost
        {
            get { return Get<decimal>("Rent.ElectricityUnit", 30.28); }

        }
        public static string RentDueDate
        {

            get { return Get("Rent.DueDate", (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10).ToString("dd/MM/yyyy"))); }

        }
        public static string RentSecurityPossessionDueDate
        {

            get { return Get("Rent.SecurityPossessionDueDate", (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10).ToString("dd/MM/yyyy"))); }

        }
        public static string Gstac
        {
            get { return Get("Taxes.GSTAC"); }

        }
        public static string GstServicesac
        {
            get { return Get("Taxes.GSTServicesAC"); }

        }
        public static decimal AdvanceTax
        {
            get { return Get<decimal>("Taxes.AdvanceTax", 0.1); }

        }

        public static decimal Wht
        {
            get { return Get<decimal>("Taxes.WHT", 0.0); }

        }

        public static decimal GSTOnSaleInvoice
        {
            get { return Get<decimal>("Taxes.GSTOnSaleInvoice", 0.0); }

        }

        public static decimal ImportMultiple
        {
            get { return Get<decimal>("Taxes.ImportMultiple", 0.00); }
        }


        public static string Customer
        {
            get { return Get("COA.Customers"); }
        }
        public static string Supplier
        {
            get { return Get("COA.Suppliers"); }
        }
        public static string Employee
        {
            get { return Get("COA.Employees"); }
        }
        public static string Products
        {
            get { return Get("COA.Products"); }
        }
        public static string Equipments
        {
            get { return Get("COA.Equipments"); }
        }
        public static string SiteEquipments
        {
            get { return Get("COA.SiteEquipments"); }
        }
        public static string InHouseEquipments
        {
            get { return Get("COA.InHouseEquipments"); }
        }
        public static string ExternalEquipments
        {
            get { return Get("COA.ExternalEquipments"); }
        }
        public static string EquipmentAc
        {
            get { return Get("COA.EquipmentAC"); }
        }
        public static string Sites
        {
            get { return Get("COA.Sites"); }
        }
        public static string Salesman
        {
            get { return Get("COA.Salesman"); }
        }

        public static string MiscCharges
        {
            get { return Get("COA.MiscCharges"); }
        }
        public static string Tenant
        {
            get { return Get("COA.Tenants"); }
        }
        public static string OrderTakers
        {
            get { return Get("COA.OrderTakers"); }
        }
        public static string TerritoryManagers
        {
            get { return Get("COA.TerritoryManagers"); }
        }
        public static string AccountType
        {
            get { return Get("COA.AccountTypes"); }
        }
        public static string CashInHandAc
        {
            get { return Get("COA.CashInHand"); }
        }
        public static string CashHeadAc
        {
            get { return Get("COA.CashHead"); }
        }
        public static string MiscHeadAc
        {
            get { return Get("COA.MiscChargesHead"); }
        }

        public static string LabourHeadsAc
        {
            get { return Get("COA.Labours"); }
        }
        public static string DemeragesHeadsAc
        {
            get { return Get("COA.Demerages"); }
        }

        public static string ExpensesHeadAc
        {
            get { return Get("COA.ExpensesHeadAc"); }
        }
        public static string IndirectExpensesHeadAc
        {
            get { return Get("COA.IndriectExpensesHeadAc"); }
        }
        public static string WorkInProcess
        {
            get { return Get("COA.WorkInProcess"); }
        }
        public static string FinishedProduction
        {
            get { return Get("COA.FinishedProduction"); }
        }
        public static string BankHeadAc
        {
            get { return Get("COA.Banks"); }
        }
        public static string BankAc
        {
            get { return Get("COA.BankAccountId"); }
        }
        public static string CurrencyAdjustmentAc
        {
            get { return Get("COA.CurrencyAdjustmentAc"); }
        }
        public static string SurchargeAc
        {
            get { return Get("COA.SurchargeAc"); }
        }
        public static string AdjustmentAc
        {
            get { return Get("COA.AdjustmentAC"); }
        }
        public static string SaleAc
        {
            get { return Get("COA.SaleAC"); }
        }
        public static string PenaltyAc
        {
            get { return Get("COA.PenaltyAC"); }
        }
        public static string TradeInAc
        {
            get { return Get("COA.TradeInAC"); }
        }
        public static string SaleReturnAc
        {
            get { return Get("COA.SaleReturnAC"); }
        }
        public static string PurchaseAc
        {
            get { return Get("COA.PurchaseAC"); }
        }
        public static string PurchaseReturnAc
        {
            get { return Get("COA.PurchaseReturnAC"); }
        }
        public static string GstSaleAc
        {
            get { return Get("COA.GSTSaleAC"); }
        }
        public static string GstSaleReturnAc
        {
            get { return Get("COA.GSTSaleReturnAC"); }
        }
        public static string GstPurchaseAc
        {
            get { return Get("COA.GSTPurchaseAC"); }
        }
        public static string GstPurchaseReturnAc
        {
            get { return Get("COA.GSTPurchaseReturnAC"); }
        }
        public static string OtherIncome
        {
            get { return Get("COA.OtherIncome"); }
        }
        public static string ServiceExpensesAc
        {
            get { return Get("COA.ServiceExpensesAC"); }
        }
        public static string SiteAc
        {
            get { return Get("COA.SiteAC"); }
        }
        public static string LabourHeadAc
        {
            get { return Get("COA.LabourHeadAc"); }
        }
        public static string DemeragesHeadAc
        {
            get { return Get("COA.DemeragesHeadAc"); }
        }
        public static string RentalIncomeAc
        {
            get { return Get("COA.RentalIncomeAC"); }
        }
        public static string StockConsumptionAc
        {
            get { return Get("COA.StockConsumptionAC"); }
        }
        public static string StockValueType
        {
            get { return Get("Stock.StockValueType"); }
        }
        public static string DeletePassword
        {
            get { return Get("Application.DeletePassword", "321"); }
        }
        public static string ApprovedBy
        {
            get { return Get("Application.ApprovedBy", "Approved By"); }
        }
        public static bool RequiredDeletePassword
        {
            get { return Get<bool>("Application.RequireDeletePassword", true); }
        }
        public static string Discount
        {
            get { return Get("COA.Discount"); }
        }
        public static string Wheat
        {
            get { return Get("COA.Wheat"); }
        }
        public static string WeighBridge
        {
            get { return Get("COA.WeighBridge"); }
        }
        public static string WheatTrade
        {
            get { return Get("COA.WheatTrade"); }
        }
        public static string BardanaJute
        {
            get { return Get("COA.BardanaJute"); }
        }
        public static string BardanaPlastic
        {
            get { return Get("COA.BardanaPlastic"); }
        }
        public static string Cutting
        {
            get { return Get("COA.Cutting"); }
        }
        public static string Loading
        {
            get { return Get("COA.Loading"); }
        }
        public static string Carriage
        {
            get { return Get("COA.Carriage"); }
        }
        public static string Whtac
        {
            get { return Get("COA.WHTAC"); }
        }
        public static string Vehicle
        {
            get { return Get("COA.Vehicle"); }
        }
        public static string WheatPurchaseSupplierAc
        {
            get { return Get("COA.WheatPurchaseHeadId"); }
        }
        public static string Freight
        {
            get { return Get("COA.Freight"); }
        }

        public static string GovtJuteAc
        {
            get { return Get("COA.GovtJuteHeadId"); }
        }
        public static string GovtPlasticAc
        {
            get { return Get("COA.GovtPlasticHeadId"); }
        }
        public static string FoodDepartmentAc
        {
            get { return Get("COA.FoodDepartmentHeadId"); }
        }
        public static string GovtWheatAc
        {
            get { return Get("COA.GovtWheatHeadId"); }
        }
        public static string ServicesAc
        {
            get { return Get("COA.ServicesAcId"); }
        }
        public static string MaterialServicesAc
        {
            get { return Get("COA.MaterialServicesAcId"); }
        }
        public static string ServicesAcccount
        {
            get { return Get("COA.Services"); }
        }
        public static string MachineAc
        {
            get { return Get("COA.Machine"); }
        }
        public static string AITAc
        {
            get { return Get("COA.AIT"); }
        }
        public static string CDAc
        {
            get { return Get("COA.CD"); }
        }
        public static string RDAc
        {
            get { return Get("COA.RD"); }
        }
        public static string SEDAc
        {
            get { return Get("COA.SED"); }
        }
        public static string FWDAc
        {
            get { return Get("COA.FWD"); }
        }
        public static string OthersAc
        {
            get { return Get("COA.Others"); }
        }
        public static string DutiesExpenseAc
        {
            get { return Get("COA.DutiesExpense"); }
        }
        public static string DailyRevenueAc
        {
            get { return Get("COA.DailyRevenue"); }
        }
        public static string RevenuHeadAc
        {
            get { return Get("COA.RevenuHeadAc"); }
        }
        public static string DailyExpenseAc
        {
            get { return Get("COA.DailyExpenseAc"); }
        }
        public static string BLExpenseHeadAc
        {
            get { return Get("COA.BLExpensesHeadAc"); }
        }
        public static string ProjectHeadAc
        {
            get { return Get("COA.ProjectHeadAc"); }
        }
        public static string CreditCardSaleAc
        {
            get { return Get("COA.CreditCardSale"); }
        }
        public static decimal Comission
        {
            get { return Get<decimal>("Attendee.Comission", 0.0); }
        }
        public static string ComissionAc
        {
            get { return Get("COA.ComissionAC"); }
        }
        public static string AdvanceTaxac
        {
            get { return Get("Taxes.AdvanceTaxAC"); }

        }
        public static string PostDatedCheques
        {
            get { return Get("COA.PostDatedCheques"); }
        }
        public static string UnpresentedCheque
        {
            get { return Get("COA.UnpresentedCheque"); }
        }
        public static string HiddenAccounts
        {
            get { return Get("COA.HiddenAccounts"); }
        }
        public static string TrackerPurchaseAc
        {
            get { return Get("COA.TrackerPurchaseAc"); }
        }
        public static string TrackerSaleAc
        {
            get { return Get("COA.TrackerSaleAc"); }
        }
        public static string InsuranceSaleAc
        {
            get { return Get("COA.InsuranceAc"); }
        }
        public static string InsurancePurchaseAc
        {
            get { return Get("COA.InsurancePurchaseAc"); }
        }
        public static string LogBookAc
        {
            get { return Get("COA.LogBookAc"); }
        }
        public static string DepriciationAc
        {
            get { return Get("COA.DepriciationAc"); }
        }
        public static int LogBookAcAcccountId
        {
            get { return GetByTitle(LogBookAc); }
        }
        public static int DepriciationAcccountId
        {
            get { return GetByTitle(DepriciationAc); }
        }
        public static int TrackerPurchaseAcccountId
        {
            get { return GetByTitle(TrackerPurchaseAc); }
        }
        public static int TrackerSaleAcccountId
        {
            get { return GetByTitle(TrackerSaleAc); }
        }
        public static int InsuranceSaleAccountId
        {
            get { return GetByTitle(InsuranceSaleAc); }
        }
        public static int InsurancePurchaseAccountId
        {
            get { return GetByTitle(InsurancePurchaseAc); }
        }
        public static int PostDatedChequesHeadId
        {
            get { return GetByTitle(PostDatedCheques); }
        }
        public static int UnpresentedChequeHeadId
        {
            get { return GetByTitle(UnpresentedCheque); }
        }

        public static int AdvanceTaxacHeadId
        {
            get { return GetByTitle(AdvanceTaxac); }
        }

        public static int ComissionAccountId
        {
            get { return GetByTitle(ComissionAc); }
        }
        public static int CreditCardSaleHeadId
        {
            get { return GetByTitle(CreditCardSaleAc); }
        }
        public static int DailyRevenueHeadId
        {
            get { return GetByTitle(DailyRevenueAc); }
        }
        public static int DailyExpenseACHeadId
        {
            get { return GetByTitle(DailyExpenseAc); }
        }
        public static int BLExpenseHeadId
        {
            get { return GetByTitle(BLExpenseHeadAc); }
        }
        public static int ProjectHeadId
        {
            get { return GetByTitle(ProjectHeadAc); }
        }


        public static int DutiesExpenseHeadId
        {
            get { return GetByTitle(DutiesExpenseAc); }
        }
        public static int AITHeadId
        {
            get { return GetByTitle(AITAc); }
        }
        public static int CDHeadId
        {
            get { return GetByTitle(CDAc); }
        }
        public static int RDHeadId
        {
            get { return GetByTitle(RDAc); }
        }
        public static int SEDHeadId
        {
            get { return GetByTitle(SEDAc); }
        }
        public static int FWDHeadId
        {
            get { return GetByTitle(FWDAc); }
        }
        public static int OthersHeadId
        {
            get { return GetByTitle(OthersAc); }
        }
        public static int MachineHeadId
        {
            get { return GetByTitle(MachineAc); }
        }
        public static int ServicesHeadId
        {
            get { return GetByTitle(ServicesAcccount); }
        }
        public static int ServicesAccountId
        {
            get { return GetByTitle(ServicesAc); }
        }
        public static int MaterialServicesAccountId
        {
            get { return GetByTitle(MaterialServicesAc); }
        }
        public static int GovtWheatHeadId
        {
            get { return GetByTitle(GovtWheatAc); }
        }
        public static int FoodDepartmentHeadId
        {
            get { return GetByTitle(FoodDepartmentAc); }
        }
        public static int GovtJuteHeadId
        {
            get { return GetByTitle(GovtJuteAc); }
        }
        public static int GovtPlasticHeadId
        {
            get { return GetByTitle(GovtPlasticAc); }
        }
        public static int FreightHeadId
        {
            get { return GetByTitle(Freight); }
        }
        public static int WheatPurchaseSupplierHeadId
        {
            get { return GetByTitle(WheatPurchaseSupplierAc); }
        }
        public static int CuttingHeadId
        {
            get { return GetByTitle(Cutting); }
        }
        public static int LoadingHeadId
        {
            get { return GetByTitle(Loading); }
        }
        public static int CarriageHeadId
        {
            get { return GetByTitle(Carriage); }
        }
        public static int WhtacHeadId
        {
            get { return GetByTitle(Whtac); }
        }
        public static int GstacHeadId
        {
            get { return GetByTitle(Gstac); }
        }
        public static int GstServicesHeadId
        {
            get { return GetByTitle(GstServicesac); }
        }
        public static int WheatHeadId
        {
            get { return GetByTitle(Wheat); }
        }
        public static int WeighBridgeHeadId
        {
            get { return GetByTitle(WeighBridge); }
        }
        public static int WheatTradeHeadId
        {
            get { return GetByTitle(WheatTrade); }
        }
        public static int BardanaJuteHeadId
        {
            get { return GetByTitle(BardanaJute); }
        }
        public static int BardanaPlasticHeadId
        {
            get { return GetByTitle(BardanaPlastic); }
        }
        public static int CustomerHeadId
        {
            get { return GetByTitle(Customer); }
        }
        public static int AssetHeadId
        {
            get { return GetByTitle(Assets); }
        }
        public static int EmployeeHeadId
        {
            get { return GetByTitle(Employee); }
        }
        public static int SupplierHeadId
        {
            get { return GetByTitle(Supplier); }
        }
        public static int ProductHeadId
        {
            get { return GetByTitle(Products); }
        }
        public static int EquipmentHeadId
        {
            get { return GetByTitle(Equipments); }
        }
        public static int SiteEquipmentHeadId
        {
            get { return GetByTitle(SiteEquipments); }
        }
        public static int InHouseEquipmentHeadId
        {
            get { return GetByTitle(InHouseEquipments); }
        }
        public static int ExternalEquipmentHeadId
        {
            get { return GetByTitle(ExternalEquipments); }
        }
        public static int EquipmentAccountId
        {
            get { return GetByTitle(EquipmentAc); }
        }
        public static int SiteHeadId
        {
            get { return GetByTitle(Sites); }
        }
        public static int SalemanHeadId
        {
            get { return GetByTitle(Salesman); }
        }
        public static int MiscChargesHeadId
        {
            get { return GetByTitle(MiscCharges); }
        }
        public static int TenantHeadId
        {
            get { return GetByTitle(Tenant); }
        }
        public static int OrderTakerHeadId
        {
            get { return GetByTitle(OrderTakers); }
        }
        public static int TerritoryManagerHeadId
        {
            get { return GetByTitle(TerritoryManagers); }
        }
        public static int CashAccountId
        {
            get { return GetByTitle(CashInHandAc); }
        }
        public static int CashHeadId
        {
            get { return GetByTitle(CashHeadAc); }
        }
        public static int MiscHeadId
        {
            get { return GetByTitle(MiscHeadAc); }
        }


        public static int WorkInProcessHeadId
        {
            get { return GetByTitle(WorkInProcess); }
        }
        public static int FinishedProductionHeadId
        {
            get { return GetByTitle(FinishedProduction); }
        }
        public static int DiscountAccountId
        {
            get { return GetByTitle(Discount); }
        }
        public static int BankHeadId
        {
            get { return GetByTitle(BankHeadAc); }
        }
        public static int BankAccountId
        {
            get { return GetByTitle(BankAc); }
        }
        public static int CurrencyAdjustmentAccountId
        {
            get { return GetByTitle(CurrencyAdjustmentAc); }
        }
        public static int SurchargeAccountId
        {
            get { return GetByTitle(SurchargeAc); }
        }
        public static int AdjustmentHeadId
        {
            get { return GetByTitle(AdjustmentAc); }
        }
        public static int SaleAccountHeadId
        {
            get { return GetByTitle(SaleAc); }
        }
        public static int PenaltyHeadId
        {
            get { return GetByTitle(PenaltyAc); }
        }
        public static int TradeInAcccountId
        {
            get { return GetByTitle(TradeInAc); }
        }


        public static int SaleReturnAccountHeadId
        {
            get { return GetByTitle(SaleReturnAc); }
        }
        public static int PurchaseAccountHeadId
        {
            get { return GetByTitle(PurchaseAc); }
        }
        public static int LabourHeadAcId
        {
            get { return GetByTitle(LabourHeadAc); }
        }
        public static int DemeragesHeadAcId
        {
            get { return GetByTitle(DemeragesHeadAc); }
        }
        public static int LabourHeadAcHeadId
        {
            get { return GetByTitle(LabourHeadsAc); }
        }
        public static int DemeragesHeadAcHeadId
        {
            get { return GetByTitle(DemeragesHeadsAc); }
        }
        public static int PurchaseReturnAccountHeadId
        {
            get { return GetByTitle(PurchaseReturnAc); }
        }
        public static int GstSaleAccountHeadId
        {
            get { return GetByTitle(GstSaleAc); }
        }
        public static int GstSaleReturnAccountHeadId
        {
            get { return GetByTitle(GstSaleReturnAc); }
        }
        public static int GstPurchaseAccountHeadId
        {
            get { return GetByTitle(GstPurchaseAc); }
        }
        public static int GstPurchaseReturnAccountHeadId
        {
            get { return GetByTitle(GstPurchaseReturnAc); }
        }
        public static int OtherIncomeAccountId
        {
            get { return GetByTitle(OtherIncome); }
        }
        public static int ServiceExpensesHeadId
        {
            get { return GetByTitle(ServiceExpensesAc); }
        }
        public static int SiteAccountHeadId
        {
            get { return GetByTitle(SiteAc); }
        }
        public static int RentalIncomeHeadId
        {
            get { return GetByTitle(RentalIncomeAc); }
        }
        public static int StockConsumptionAccountId
        {
            get { return GetByTitle(StockConsumptionAc); }
        }

        public static int GstHeadId
        {
            get { return GetByTitle(Gstac); }
        }
        public static int RentId
        {
            get { return GetByTitle(Get("COA.Rent")); }
        }
        public static int UtitlityChargesId
        {
            get { return GetByTitle(Get("COA.UtitlityCharges")); }
        }
        public static int ElectricityId
        {
            get { return GetByTitle(Get("COA.Electricity")); }
        }
        public static int PossessionChargesId
        {
            get { return GetByTitle(Get("COA.PossessionCharges")); }
        }
        public static int TfrFeeId
        {
            get { return GetByTitle(Get("COA.TfrFee")); }
        }
        public static int SecurityMoneyId
        {
            get { return GetByTitle(Get("COA.SecurityMoney")); }
        }
        public static int PromoActivityId
        {
            get { return GetByTitle(Get("COA.PromoActivity")); }
        }
        public static int PromotionAcccountId
        {
            get { return GetByTitle(Get("COA.PromotionAc")); }
        }
        public static int CarParkingId
        {
            get { return GetByTitle(Get("COA.CarParking")); }
        }
        public static int BankProfitId
        {
            get { return GetByTitle(Get("COA.BankProfit")); }
        }
        public static int SurchargeId
        {
            get { return GetByTitle(Get("COA.Surcharge")); }
        }
        public static int MiscId
        {
            get { return GetByTitle(Get("COA.Misc")); }
        }
        public static int CrsId
        {
            get { return GetByTitle(Get("COA.CRs")); }
        }
        public static int DrsId
        {
            get { return GetByTitle(Get("COA.DRs")); }
        }
        public static int VehicleHeadId
        {
            get { return GetByTitle(Vehicle); }
        }
        public static int RentPerSqft
        {
            get { return Get<int>("Rent.RentPerSqft"); }
        }
        public static int UCPercent
        {
            get { return Get<int>("Rent.UCPercent"); }
        }
        public static int PossessionChargesPerSqft
        {
            get { return Get<int>("Rent.PossessionChargesPerSqft"); }
        }
        public static int SecurityMoneyMonths
        {
            get { return Get<int>("Rent.SecurityMoneyMonths"); }
        }
        public static int TransferCharges
        {
            get { return Get<int>("Rent.TransferCharges"); }
        }

        #endregion

        #region  Voucher Base  Settings
        public static string BVAccountant
        {
            get { return Get("B.V.AccountantText", "Accountant"); }
        }
        public static string BVCreatedBy
        {
            get { return Get("B.V.CreatedByText", "Created By"); }
        }
        public static string BVApprovedBy
        {
            get { return Get("B.V.ApprovedByText", "Approved By"); }
        }
        public static string BVReceivedBy
        {
            get { return Get("B.V.ReceivedByText", "Received By"); }
        }
        public static string CVAccountant
        {
            get { return Get("C.V.AccountantText", "Accountant"); }
        }
        public static string CVCreatedBy
        {
            get { return Get("C.V.CreatedByText", "Created By"); }
        }
        public static string CVApprovedBy
        {
            get { return Get("C.V.ApprovedByText", "Approved By"); }
        }
        public static string CVReceivedBy
        {
            get { return Get("C.V.ReceivedByText", "Received By"); }
        }
        public static string JVAccountant
        {
            get { return Get("J.V.AccountantText", "Accountant"); }
        }
        public static string JVApprovedBy
        {
            get { return Get("J.V.ApprovedByText", "Approved By"); }
        }


        public static string JVReceivedBy
        {
            get { return Get("J.V.RecievedByText", "Recieved By"); }
        }


        #endregion

        #region Salary

        public static string SalaryExpenseAc
        {
            get { return Get("COA.SalaryExpense"); }
        }
        public static string MonthlyRentAc
        {
            get { return Get("COA.MonthlyRent"); }
        }
        public static string UtliliychargeAc
        {
            get { return Get("COA.Utliliycharge"); }
        }
        public static string ElectricitychargeAc
        {
            get { return Get("COA.Electricitycharge"); }
        }
        public static string PFAc
        {
            get { return Get("COA.ProvidentFund"); }
        }
        public static string EOBIAc
        {
            get { return Get("COA.EOBI"); }
        }
        public static string SSTAc
        {
            get { return Get("COA.SST"); }
        }
        public static string IncomeTaxAc
        {
            get { return Get("COA.IncomeTax"); }
        }


        public static int SalaryExpenseId
        {
            get { return GetByTitle(SalaryExpenseAc); }
        }
        public static int MonthlyRentAcId
        {
            get { return GetByTitle(MonthlyRentAc); }
        }
        public static int UtliliychargeAcId
        {
            get { return GetByTitle(UtliliychargeAc); }
        }
        public static int ElectricitychargeAcId
        {
            get { return GetByTitle(ElectricitychargeAc); }
        }
        public static int PFAccountId
        {
            get { return GetByTitle(PFAc); }
        }
        public static int EOBIId
        {
            get { return GetByTitle(EOBIAc); }
        }
        public static int SSTId
        {
            get { return GetByTitle(SSTAc); }
        }
        public static int IncomeTaxId
        {
            get { return GetByTitle(IncomeTaxAc); }
        }

        #endregion

        #region PLS & Fiscal

        public static string PLSAc
        {
            get { return Get("COA.PLS"); }
        }
        public static string UnappropriatedProfitAc
        {
            get { return Get("COA.UnappropriatedProfit"); }
        }
        public static string StockValueAc
        {
            get { return Get("COA.StockValue"); }
        }
        public static string CGSAc
        {
            get { return Get("COA.CGS"); }
        }
        public static string ExpensesAc
        {
            get { return Get("COA.ExpensesAc"); }
        }
        public static string DirectExpensesHeadAc
        {
            get { return Get("COA.DirectExpensesHeadAc"); }
        }


        public static int PLSAccountId
        {
            get { return GetByTitle(PLSAc); }
        }
        public static int UnappropriatedProfitAccountId
        {
            get { return GetByTitle(UnappropriatedProfitAc); }
        }
        public static int StockValueAccountId
        {
            get { return GetByTitle(StockValueAc); }
        }
        public static int CGSAccountId
        {
            get { return GetByTitle(CGSAc); }
        }
        public static int ExpensesAccountId
        {
            get { return GetByTitle(ExpensesAc); }
        }
        public static int ExpensesHeadId
        {
            get { return GetByTitle(ExpensesHeadAc); }
        }
        public static int RevenuHeadId
        {
            get { return GetByTitle(RevenuHeadAc); }
        }
        public static int DirectExpensesHeadId
        {
            get { return GetByTitle(DirectExpensesHeadAc); }
        }

        public static int InDirectExpensesHeadId
        {
            get { return GetByTitle(IndirectExpensesHeadAc); }
        }


        #endregion

        #region Companies

        public static int DRCRBalanceinDecimalGeneralLedger
        {
            get { return Get<int>("Application.DRCRBalanceinDecimalGeneralLedger", 2); }
        }
        public static bool IsProductTDSAndMSDS
        {
            get { return Get<bool>("Product.ProductTDSAndMSDSHide", false); }
        }

        public static bool IsFiscalYearShowandHide
        {
            get { return Get<bool>("Company.FiscalYearShow/Hide", false); }
        }
        public static bool IsCompanyLogoDetailsShow
        {
            get { return Get<bool>("Application.Reports.CompanyLogoDetailsShow", true); }
        }

        public static string Address
        {
            get { return Get("CONTACTS.Address", ""); }
        }
        public static string License
        {
            get { return Get("CONTACTS.License", ""); }
        }
        public static string CityState
        {
            get { return Get("CONTACTS.CityState", ""); }
        }
        public static string Phone
        {
            get { return Get("CONTACTS.Phone", ""); }
        }
        public static string Phone1
        {
            get { return Get("CONTACTS.Phone1", ""); }
        }
        public static string Fax
        {
            get { return Get("CONTACTS.Fax", ""); }
        }
        public static string Email
        {
            get { return Get("CONTACTS.Email", ""); }
        }

        public static string Web
        {
            get { return Get("CONTACTS.Web", ""); }
        }
        public static string RSS
        {
            get { return Get("Follow.RSS", ""); }
        }
        public static string Facebook
        {
            get { return Get("Follow.Facebook", ""); }
        }
        public static string Twitter
        {
            get { return Get("Follow.Twitter", "javascript:void(0);"); }
        }
        public static string Gmail
        {
            get { return Get("Follow.Gmail", "javascript:void(0);"); }
        }
        public static string LinkedIn
        {
            get { return Get("Follow.LinkedIn", "javascript:void(0);"); }
        }
        public static string Youtube
        {
            get { return Get("Follow.Youtube", "javascript:void(0);"); }
        }
        public static string SubscribeEmail
        {
            get { return Get("Application.Email", ""); }
        }
        public static string NTN
        {
            get { return Get("Application.NTN", ""); }
        }
        public static string SaleTaxRegNo
        {
            get { return Get("Application.SaleTaxRegNo", ""); }
        }
        public static string BankAcNo
        {
            get { return Get("Application.BankAccountNo", " "); }
        }
        public static string About
        {
            get { return Get("Application.About", ""); }
        }

        public static string SaleTaxInvoiceFirstHeaderText
        {
            get { return Get("Sale.SaleTaxInvoiceFirstHeaderText", ""); }
        }
        public static string SaleTaxInvoiceSecondHeaderText
        {
            get { return Get("Sale.SaleTaxInvoiceSecondHeaderText", ""); }
        }
        public static string SaleTaxInvoiceNTNNoAndSTRNText
        {
            get { return Get("Sale.SaleTaxInvoiceNTNNoAndSTRNText", ""); }
        }



        public static string WarrentyInvoiceText
        {
            get { return Get("Invoice.WarrentyInvoiceText", ""); }
        }
        #endregion

        #region Urls

        public static string DashBoardUrl
        {
            get { return Get("Other.DashBoardUrl", "~/home/xdashboard?name=AdminDashboard"); }
        }
        public static string SalePrintUrl
        {
            get { return Get("Url.SalePrintUrl"); }
        }
        public static string SaleJsFunction
        {
            get { return Get("Url.SaleJsFunction"); }
        }
        public static string SaleReturnPrintUrl
        {
            get { return Get("Url.SaleReturnPrintUrl"); }
        }
        public static string SaleReturnJsFunction
        {
            get { return Get("Url.SaleReturnJsFunction"); }
        }


        public static string PurchasePrintUrl
        {
            get { return Get("Url.PurchasePrintUrl"); }
        }
        public static string PurchaseJsFunction
        {
            get { return Get("Url.PurchaseJsFunction"); }
        }
        public static string PurchaseReturnPrintUrl
        {
            get { return Get("Url.PurchaseReturnPrintUrl"); }
        }
        public static string PurchaseReturnJsFunction
        {
            get { return Get("Url.PurchaseReturnJsFunction"); }
        }

        public static string ServicePrintUrl
        {
            get { return Get("Url.ServicePrintUrl"); }
        }
        public static string ServiceJsFunction
        {
            get { return Get("Url.ServiceJsFunction"); }
        }

        public static string CashReceiptPrintUrl
        {
            get { return Get("Url.CashReceiptPrintUrl"); }
        }
        public static string CashReceiptJsFunction
        {
            get { return Get("Url.CashReceiptJsFunction"); }
        }
        public static string CashPaymentPrintUrl
        {
            get { return Get("Url.CashPaymentPrintUrl"); }
        }
        public static string CashPaymentJsFunction
        {
            get { return Get("Url.CashPaymentJsFunction"); }
        }



        public static string BankReceiptPrintUrl
        {
            get { return Get("Url.BankReceiptPrintUrl"); }
        }
        public static string BankReceiptJsFunction
        {
            get { return Get("Url.BankReceiptJsFunction"); }
        }
        public static string BankPaymentPrintUrl
        {
            get { return Get("Url.BankPaymentPrintUrl"); }
        }
        public static string BankPaymentJsFunction
        {
            get { return Get("Url.BankPaymentJsFunction"); }
        }



        public static string TransferVoucherPrintUrl
        {
            get { return Get("Url.TransferVoucherPrintUrl"); }
        }
        public static string TransferVoucherJsFunction
        {
            get { return Get("Url.TransferVoucherJsFunction"); }
        }



        public static string SalaryVoucherPrintUrl
        {
            get { return Get("Url.SalaryVoucherPrintUrl"); }
        }
        public static string SalaryVoucherJsFunction
        {
            get { return Get("Url.SalaryVoucherJsFunction"); }
        }

        #endregion

        #region Misc
        public static bool IsVehicleCompnay
        {
            get { return Get<bool>("Application.IsVehicleCompany", false); }
        }
        public static bool AllowCGS
        {
            get { return Get<bool>("Application.AllowCGS", false); }
        }

        public static bool CheckStockAvailability
        {
            get { return Get<bool>("Application.CheckStockAvailability", false); }
        }

        public static bool DuplicateBatchNoAllowedOnPurchase
        {
            get { return Get<bool>("Purchase.DuplicateBatchNoAllowed", false); }
        }


        public static bool DuplicateBatchNoAllowedOnProduction
        {
            get { return Get<bool>("Production.DuplicateBatchNoAllowed", false); }
        }

        public static bool BatchWiseConsumingRawMaterialAllowedOnProduction
        {
            get { return Get<bool>("Production.BatchWiseConsumingRawMaterialAllowed", false); }
        }

        public static string AllowBatchandExpiryDate
        {
            get { return Get<string>("ValidationAllForms.RequiredBatchNoandExpiryDate", "false"); }
        }

        public static string AllowLocalORImportGST
        {
            get { return Get<string>("Purchase.LocalORImportGST", "Import"); }
        }


        public static bool CheckBatchWiseStockAvailability
        {
            get { return Get<bool>("Application.CheckBatchWiseStockAvailability", false); }
        }
        public static bool LockSaleForecast
        {
            get { return Get<bool>("Application.LockSaleForecast", false); }
        }
        #endregion
        #region Partners
        public static bool IsCashRecieptAllowPartner
        {
            get { return Get<bool>("VOUCHER.CASHRECEIPT.ALLOWPARTNER", false); }
        }
        public static bool IsCashPaymentAllowPartner
        {
            get { return Get<bool>("VOUCHER.CASHPAYMENT.ALLOWPARTNER", false); }
        }
        public static bool IsCashRecieptAllowBillingMonth
        {
            get { return Get<bool>("Voucher.CashReciept.AllowBillingMonth", false); }
        }
        public static bool IsBankRecieptAllowPartner
        {
            get { return Get<bool>("VOUCHER.BANKRECEIPT.ALLOWPARTNER", false); }
        }
        public static bool IsBankRecieptAllowBillingMonth
        {
            get { return Get<bool>("Voucher.BankReciept.AllowBillingMonth", false); }
        }
        public static bool IsBankPaymentAllowPartner
        {
            get { return Get<bool>("VOUCHER.BANKPAYMENT.ALLOWPARTNER", false); }
        }


        public static bool IsJVAllowPartner
        {
            get { return Get<bool>("VOUCHER.JOURNALVOUCHER.ALLOWPARTNER", false); }
        }
        public static bool IsJVAllowCloning
        {
            get { return Get<bool>("VOUCHER.JOURNALVOUCHER.IsJVAllowCloning", false); }
        }


        public static bool IsGSTSaleAllowPartner
        {
            get { return Get<bool>("TRANSACTION.GSTSALE.ALLOWPARTNER", false); }
        }
        public static bool IsGSTSaleReturnAllowPartner
        {
            get { return Get<bool>("TRANSACTION.GSTSALERETURN.ALLOWPARTNER", false); }
        }
        public static bool IsGSTPurchaseAllowPartner
        {
            get { return Get<bool>("TRANSACTION.GSTPURCHASE.ALLOWPARTNER", false); }
        }
        public static bool IsGSTPurchaseReturnAllowPartner
        {
            get { return Get<bool>("TRANSACTION.GSTPURCHASERETURN.ALLOWPARTNER", false); }
        }

        public static bool IsCommercialSaleAllowPartner
        {
            get { return Get<bool>("TRANSACTION.COMMERCIALSALE.ALLOWPARTNER", false); }
        }
        public static string CommercialSaleText
        {
            get { return Get<string>("TRANSACTION.COMMERCIALSALE.InvoicesText", "Sales Invoice"); }
        }
        public static bool IsCommercialSaleReturnAllowPartner
        {
            get { return Get<bool>("TRANSACTION.COMMERCIALSALERETURN.ALLOWPARTNER", false); }
        }

        public static bool IsCommercialSaleReturnAllowMultipleItem
        {
            get { return Get<bool>("TRANSACTION.COMMERCIALSALERETURN.MultipleItemonSR", false); }
        }

        public static bool IsCommercialSaleAllowMultipleItem
        {
            get { return Get<bool>("TRANSACTION.CommercialSale.MultipleItemonSaleVoucher", false); }
        }
        public static bool IsCommercialPurchaseAllowPartner
        {
            get { return Get<bool>("TRANSACTION.COMMERCIALPURCHASE.ALLOWPARTNER", false); }
        }
        public static bool IsCommercialPurchaseReturnAllowPartner
        {
            get { return Get<bool>("TRANSACTION.COMMERCIALPURCHASERETURN.ALLOWPARTNER", false); }
        }
        #endregion

        #region Vouchers
        public static bool IsCashRecieptAllowVehicle
        {
            get { return Get<bool>("VOUCHER.CASHRECEIPT.allowVehicle", false); }
        }
        public static bool IsCashPaymentAllowVehicle
        {
            get { return Get<bool>("VOUCHER.CASHPAYMENT.allowVehicle", false); }
        }
        public static bool IsBankRecieptAllowVehicle
        {
            get { return Get<bool>("VOUCHER.BANKRECEIPT.allowVehicle", false); }
        }
        public static bool IsBankPaymentAllowVehicle
        {
            get { return Get<bool>("VOUCHER.BANKPAYMENT.allowVehicle", false); }
        }


        public static bool IsCashRecieptAllowCostCenter
        {
            get { return Get<bool>("VOUCHER.CASHRECEIPT.ALLOWCOSTCENTER", false); }
        }
        public static bool IsCashPaymentAllowCostCenter
        {
            get { return Get<bool>("VOUCHER.CASHPAYMENT.ALLOWCOSTCENTER", false); }
        }
        public static bool IsBankRecieptAllowCostCenter
        {
            get { return Get<bool>("VOUCHER.BANKRECEIPT.ALLOWCOSTCENTER", false); }
        }
        public static bool IsBankPaymentAllowCostCenter
        {
            get { return Get<bool>("VOUCHER.BANKPAYMENT.ALLOWCOSTCENTER", false); }
        }


        public static int CashRecieptAccountType
        {
            get { return Get<int>("VOUCHER.CASHRECEIPT.AccountType", 1); }
        }
        public static int CashPaymentAccountType
        {
            get { return Get<int>("VOUCHER.CASHPAYMENT.AccountType", 1); }
        }
        public static int BankRecieptAccountType
        {
            get { return Get<int>("VOUCHER.BANKRECEIPT.AccountType", 1); }
        }
        public static int BankPaymentAccountType
        {
            get { return Get<int>("VOUCHER.BANKPAYMENT.AccountType", 1); }
        }
        public static int AmountInWordType
        {
            get { return Get<int>("VOUCHER.AmountInWords", 1); }
        }


        #endregion
        #region Vehicles
        public static bool IsAllowVehicleValidation
        {
            get { return Get<bool>("Application.AllowVehicleValidation", false); }
        }

        public static string DeliveryChargeAccount
        {
            get { return Get("COA.Vehicle.DeliveryOrderCharges"); }
        }
        public static string PortChargesAccount
        {
            get { return Get("COA.Vehicle.PortCharges"); }
        }
        public static string FareAccount
        {
            get { return Get("COA.Vehicle.FareCharge"); }
        }
        public static string StorageChargeAccount
        {
            get { return Get("COA.Vehicle.StorageCharge"); }
        }
        public static int DeliveryChargeAccountId
        {
            get { return GetByTitle(DeliveryChargeAccount); }
        }
        public static int PortChargesAccountId
        {
            get { return GetByTitle(PortChargesAccount); }
        }
        public static int FareAccountId
        {
            get { return GetByTitle(FareAccount); }
        }
        public static int StorageChargeAccountId
        {
            get { return GetByTitle(StorageChargeAccount); }
        }

        #endregion



        #region Rate & Amount
        public static bool IsSaleOrderAllowRate
        {
            get { return Get<bool>("ORDER.SALEORDER.SHOWRATEAMOUNT", true); }
        }

        public static string SALEORDERRateName
        {
            get { return Get("SALEORDER.RateNameText", "Rate"); }
        }
        public static string PURCHASEORDERRateName
        {
            get { return Get("PURCHASEORDER.RateNameText", "Rate"); }
        }
        public static bool IsPurchaseOrderAllowRate
        {
            get { return Get<bool>("ORDER.PURCHASEORDER.SHOWRATEAMOUNT", false); }
        }
        public static bool IsGINAllowRate
        {
            get { return Get<bool>("DELIVERYCHALLAN.GIN.SHOWRATEAMOUNT", false); }
        }
        public static bool IsGRNAllowRate
        {
            get { return Get<bool>("DELIVERYCHALLAN.GRN.SHOWRATEAMOUNT", false); }
        }

        public static bool IsGRNAllowStock
        {
            get { return Get<bool>("DELIVERYCHALLAN.GRN.AllowStockOnGRN", false); }
        }
        #endregion

        #region Finalization
        public static bool IsBankReceiptAllowFinalization
        {
            get { return Get<bool>("VOUCHER.BANKRECEIPT.ALLOWFINALIZATION", false); }
        }
        public static bool IsBankPaymentAllowFinalization
        {
            get { return Get<bool>("VOUCHER.BANKPAYMENT.ALLOWFINALIZATION", false); }
        }
        public static bool IsCashPaymentAllowFinalization
        {
            get { return Get<bool>("VOUCHER.CASHPAYMENT.ALLOWFINALIZATION", false); }
        }
        public static bool IsCashReceiptAllowFinalization
        {
            get { return Get<bool>("VOUCHER.CASHRECEIPT.ALLOWFINALIZATION", false); }
        }
        public static bool IsJvAllowFinalization
        {
            get { return Get<bool>("VOUCHER.JOURNALVOUCHER.ALLOWFINALIZATION", false); }
        }

        public static bool IsUnpresentedTransactionAllowed
        {
            get { return Get<bool>("VOUCHER.IsUnpresentedTransactionAllowed", true); }
        }

        #endregion

        #region Discounts
        public static bool IsSaleAllowDisocunt
        {
            get { return Get<bool>("Transaction.CommercialSale.AllowDiscount", false); }
        }
        public static bool IsPurchaseAllowDisocunt
        {
            get { return Get<bool>("Transaction.CommercialPurchase.AllowDiscount", false); }
        }

        public static bool IsAllowShowIndustriesinRole
        {
            get { return Get<bool>("Users.AllowShowIndustriesinRole", false); }
        }


        #endregion

        #region WheatPurchase

        public static int WheatPurcaseWeighBridgeAmount
        {
            get { return Get<int>("Transaction.WheatPurcase.WeighAmount", 0); }
        }

        public static int WheatPurcaseWeighBridgeAmount1
        {
            get { return Get<int>("Transaction.WheatPurcase.WeighAmount1", 0); }
        }
        public static int WheatTradeAmount
        {
            get { return Get<int>("Transaction.WheatPurcase.WheatTradeAmount", 0); }
        }
        #endregion

        #region Reports

        public static bool AllowDynamicReportGraph
        {
            get { return Get<bool>("Reports.Dynamic.AllowGraph", true); }
        }

        #endregion




        public static void SaveSettings(List<Setting> settings)
        {
            var repo = new SettingRepository();
            using (var scope = TransactionScopeBuilder.Create(new System.TimeSpan(0, 15, 0)))
            {
                repo.Save(settings);
                repo.SaveChanges();
                scope.Complete();
            }
            SiteContext.Current.Settings = settings.ToDictionary(p => p.Key, q => q.Value);
        }
        public static int GetByTitle(string title)
        {
            var cacheKey = string.Format("AccountId_{0}_{1}", SiteContext.Current.User.CompanyId, title);
            var value = SiteContext.Current.Cache(cacheKey);
            if (value != null)
            {
                var id = Cast.To<int>(value);
                if (id > 0) return id;
            }

            var ac = new AccountRepository().GetIdByName(title);
            SiteContext.Current.Cache(cacheKey, ac);
            return ac;
        }
        public static int GetAllByTitle(string titless)
        {
            return new AccountRepository().GetIdByName(titless);
        }
        public static Dictionary<string, string> GetAll()
        {
            return SiteContext.Current.Settings ?? (SiteContext.Current.Settings = new SettingRepository().GetAll().ToDictionary(p => p.Key, q => q.Value));
        }
        public static Dictionary<string, string> Settings
        {
            get { if (SiteContext.Current.User != null) return GetAll(); else return null; }
        }

        public static void RefreshSetting()
        {
            SiteContext.Current.Settings = new SettingRepository().GetAll().ToDictionary(p => p.Key, q => q.Value);
        }
        public static void RefreshSetting(int CompanyId)
        {
            SiteContext.Current.Settings = new SettingRepository().GetByCompanyId(CompanyId).ToDictionary(p => p.Key, q => q.Value);
        }
        public static string Get(string key)
        {
            return Settings != null && Settings.ContainsKey(key) ? Settings[key] : "";
        }
        public static string Get(string key, string defaultValue)
        {
            return Settings != null && Settings.ContainsKey(key) && !string.IsNullOrWhiteSpace(Settings[key]) ? Settings[key] : defaultValue;
        }
        public static T Get<T>(string key, object defaultValue)
        {
            var value = Get(key);
            return (T)Convert.ChangeType(!string.IsNullOrEmpty(value) ? value : defaultValue, typeof(T));
        }
        public static T Get<T>(string key)
        {
            return (T)Convert.ChangeType(Get(key), typeof(T));
        }

    }
}
