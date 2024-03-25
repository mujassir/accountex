using System.Data.Entity;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.CodeFirst.Context
{
    public partial class AccountExContext : DbContext
    {

        public DbSet<vw_Transaction> vw_Transaction { get; set; }
        public DbSet<vw_Vouchers> vw_Vouchers { get; set; }
        public DbSet<vw_JVVouchers> vw_JVVouchers { get; set; }
        public DbSet<vw_PendingDeliveryChallan> vw_PendingDeliveryChallan { get; set; }
        public DbSet<vw_RentAgreements> vw_RentAgreements { get; set; }
        public DbSet<vw_GetRentAgreementsWithTenants> vw_GetRentAgreementsWithTenants { get; set; }
        public DbSet<vw_VehiclePostDateCheques> vw_VehiclePostDateCheques { get; set; }
        public DbSet<vw_VehicleVouchers> vw_VehicleVouchers { get; set; }
        public DbSet<vw_GetNextFollowUps> vw_GetNextFollowUps { get; set; }
        public DbSet<vw_TenantAgreements> vw_ElectricityUnits { get; set; }
        public DbSet<vw_VehicleSales> vw_VehicleSales { get; set; }
        public DbSet<vw_VehicleSendRequests> vw_VehicleSendRequests { get; set; }
        public DbSet<vw_SupplierProducts> vw_SupplierProducts { get; set; }
        public DbSet<vw_Vehicles> vw_Vehicles { get; set; }
        public DbSet<vw_BLS> vw_BLS { get; set; }
        public DbSet<vw_RentOpeningBalance> vw_RentOpeningBalances { get; set; }
        public DbSet<vw_VehicleStock> vw_VehicleStock { get; set; }
        public DbSet<vw_PharmacyProduct> vw_PharmacyProducts { get; set; }
        public DbSet<vw_CurrencyRate> vw_CurrencyRates { get; set; }
        public DbSet<vw_SupplierCurrency> vw_SupplierCurrencies { get; set; }
        public DbSet<vw_ProductGroups> vw_ProductGroups { get; set; }
    }
}
