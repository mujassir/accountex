using System.Data.Entity;
using AccountEx.CodeFirst.Mapping;
using AccountEx.CodeFirst.Models;
using Attribute = AccountEx.CodeFirst.Models.Attribute;
using Entities.CodeFirst;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.CodeFirst.Models.Transactions;

using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.CodeFirst.Models.Pharmaceutical;
using System.Data.Entity.ModelConfiguration.Conventions;




namespace AccountEx.CodeFirst.Context
{
    public partial class AccountExContext : DbContext
    {
        static AccountExContext()
        {
            Database.SetInitializer<AccountExContext>(null);
        }


        public AccountExContext()
            : base("Name=AccountEx")
        {
        }
        public AccountExContext(string application)
            : base("Name=" + application)
        {
        }
        //public DbSet<Company> Companies { get; set; }

        public DbSet<MedicineItem> MedicineItems { get; set; }

        public DbSet<AccountDetailForm> AccountDetailForms { get; set; }
        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankReceipt> BankReceipts { get; set; }
        public DbSet<BankReceiptItem> BankReceiptItems { get; set; }
        public DbSet<AssetType> Assettypes { get; set; }

        public DbSet<CustomerDiscount> CustomerDiscounts { get; set; }
        public DbSet<ProductMapping> ProductMappings { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<EmployeeOTHour> EmployeeOTHours { get; set; }
        public DbSet<FormSetting> FormSettings { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProjectReceipt> ProjectReceipts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseTotalItem> PurchaseTotalItems { get; set; }
        public DbSet<ReportColumn> ReportColumns { get; set; }
        public DbSet<ReportParameter> ReportParameters { get; set; }
        public DbSet<WPItem> WPItem { get; set; }
        public DbSet<WorkInProgress> WorkInProgresses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public DbSet<InvoiceClearing> InvoiceClearings { get; set; }

        public DbSet<RequisitionItem> RequisitionItems { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }

        public DbSet<VehicleBranch> VehicleBranches { get; set; }
        public DbSet<VehiclePostDatedCheque> PDCS { get; set; }


        public DbSet<VoucherItem> VoucherItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<FiscalSetting> FiscalSettings { get; set; }
        public DbSet<StorePurchaseTotalItem> StorePurchaseTotalItems { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreTransaction> StoreTransactions { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierGroup> SupplierGroups { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DairyAdjustment> DairyAdjustments { get; set; }
        public DbSet<DairyTransaction> DairyTransactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Models.Action> Actions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<WheatPurchase> WheatPurchases { get; set; }
        public DbSet<WheatPurchaseItem> WheatPurchaseItems { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<RoleAction> RoleActions { get; set; }
        public DbSet<Transporter> Transporters { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DCItem> DCItems { get; set; }
        public DbSet<DeliveryChallan> DeliveryChallans { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<SaleServicesItem> SaleServicesItems { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<LogMapping> LogMappings { get; set; }
        public DbSet<LogData> LogData { get; set; }
        public DbSet<DomainMapping> DomainMappings { get; set; }

        public DbSet<EmployeeIncomeConfig> EmployeeIncomeConfigs { get; set; }
        public DbSet<StockRequisition> StockRequisitions { get; set; }
        public DbSet<StockRequisitionItem> StockRequisitionItems { get; set; }


        public DbSet<ESalary> ESalaries { get; set; }
        public DbSet<SalaryItem> SalaryItems { get; set; }

        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<CompanyDashboard> CompanyDashboards { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<WidgetColumn> WidgetColumns { get; set; }
        public DbSet<WidgetParameter> WidgetParameters { get; set; }
        public DbSet<DashboardWidget> DashboardWidgets { get; set; }

        public DbSet<SalaryConfig> SalaryConfigs { get; set; }
        public DbSet<SalaryConfigItem> SalaryConfigItems { get; set; }

        public DbSet<ProductVersion> ProductVersions { get; set; }
        public DbSet<Fiscal> Fiscals { get; set; }
        public DbSet<OpeningBalance> OpeningBalances { get; set; }

        public DbSet<LeadConcernedItem> LeadConcernedItems { get; set; }
        public DbSet<LeadExpectedItem> LeadExpectedItems { get; set; }
        public DbSet<Lead> Leads { get; set; }

        public DbSet<LeadActivity> LeadActivities { get; set; }
        public DbSet<SaleDiscountItem> SaleDiscountItems { get; set; }
        public DbSet<SaleDiscount> SaleDiscounts { get; set; }
        public DbSet<ServiceExpense> ServiceExpenses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<OrderExpenseItem> OrderServicesItems { get; set; }

        public DbSet<ShopDocument> ShopDocuments { get; set; }

        public DbSet<RentAgreement> RentAgreements { get; set; }
        public DbSet<RentAgreementSchedule> RentAgreementSchedules { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<ElectricityUnit> ElectricityUnits { get; set; }
        public DbSet<ElectricityUnitItem> ElectricityUnitItems { get; set; }
        public DbSet<TenantPartner> TenantPartners { get; set; }
        public DbSet<MiscChargeItem> MiscChargeItems { get; set; }
        public DbSet<MiscCharge> MiscCharges { get; set; }
        public DbSet<RentDetailItem> RentDetailItems { get; set; }
        public DbSet<RentDetail> RentDetails { get; set; }
        public DbSet<Challan> Challans { get; set; }
        public DbSet<ChallanItem> ChallanItems { get; set; }
        public DbSet<VehicleStatuse> VehicleStatuses { get; set; }
        public DbSet<VehicleSale> VehicleSales { get; set; }
        public DbSet<VehicleSaleDeposit> VehicleSaleDeposits { get; set; }
        public DbSet<VehicleSaleDetail> VehicleSaleDetails { get; set; }
        public DbSet<VehicleRequest> VehicleRequests { get; set; }

        public DbSet<BL> BLs { get; set; }
        public DbSet<BLItem> BLItems { get; set; }
        public DbSet<BLStatusHistory> BLStatusHistories { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<PayablePayment> PayablePayment { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAction> NotificationActions { get; set; }
        public DbSet<NotificationJob> NotificationJobs { get; set; }
        public DbSet<NotificationJobTrigger> NotificationJobTriggers { get; set; }
        public DbSet<NotificationJobAction> NotificationJobActions { get; set; }
        public DbSet<NotificationJobActor> NotificationJobActors { get; set; }
        public DbSet<NotificationJobContact> NotificationJobContacts { get; set; }
        public DbSet<NotificationJobType> NotificationJobTypes { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }




        public DbSet<VehicleInstallmentPayment> VehicleInstallmentPayment { get; set; }
        public DbSet<VehiclePayment> VehiclePayments { get; set; }

        public DbSet<VehicleFollowUp> VehicleFollowUps { get; set; }

        public DbSet<VehicleVoucher> VehicleVouchers { get; set; }

        public DbSet<UserBranch> UserBranches { get; set; }
        public DbSet<Auctioneer> Auctioneers { get; set; }
        public DbSet<VehicleAcution> VehicleAcutions { get; set; }
        public DbSet<BLCharge> BLCharges { get; set; }
        public DbSet<VehiclePenalty> VehiclePenalties { get; set; }

        public DbSet<SaleDocument> SaleDocuments { get; set; }
        public DbSet<RentOpeningBalance> RentOpeningBalances { get; set; }

        public DbSet<VehicleLogBookScane> VehicleLogBookScanes { get; set; }
        public DbSet<UserVehicleBranch> UserVehicleBranches { get; set; }

        public DbSet<UploadDocument> Documents { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        public DbSet<SupplierCurrency> SupplierCurrencies { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<ProductReceipe> ProductReceipe { get; set; }
        public DbSet<ProductReceipeitems> ProductReceipeitems { get; set; }
        public DbSet<vw_ProductReceipes> vw_ProductReceipes { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {



            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 4));
            modelBuilder.Configurations.Add(new AccountAttributeMap());
            modelBuilder.Configurations.Add(new AccountDetailFormMap());
            modelBuilder.Configurations.Add(new AccountDetailMap());
            modelBuilder.Configurations.Add(new AccountMap());
            modelBuilder.Configurations.Add(new AccountTypeMap());
            modelBuilder.Configurations.Add(new AssetMap());
            modelBuilder.Configurations.Add(new AttributeMap());
            modelBuilder.Configurations.Add(new AttributeTypeMap());
            modelBuilder.Configurations.Add(new BankMap());
            modelBuilder.Configurations.Add(new ConfigMap());
            modelBuilder.Configurations.Add(new CustomerDiscountMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new FormSettingMap());
            modelBuilder.Configurations.Add(new MarkaMap());
            modelBuilder.Configurations.Add(new MenuItemMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProjectReceiptMap());
            modelBuilder.Configurations.Add(new ProjectMap());
            modelBuilder.Configurations.Add(new PurchaseMap());
            modelBuilder.Configurations.Add(new PurchaseTotalItemMap());
            modelBuilder.Configurations.Add(new ReportColumnMap());
            modelBuilder.Configurations.Add(new ReportParameterMap());
            modelBuilder.Configurations.Add(new ReportMap());
            modelBuilder.Configurations.Add(new SalaryMap());
            modelBuilder.Configurations.Add(new SaleItemMap());
            modelBuilder.Configurations.Add(new SaleMap());
            modelBuilder.Configurations.Add(new SettingMap());
            modelBuilder.Configurations.Add(new StorePurchaseTotalItemMap());
            modelBuilder.Configurations.Add(new StoreMap());
            modelBuilder.Configurations.Add(new StoreTransactionMap());
            modelBuilder.Configurations.Add(new SupplierMap());
            modelBuilder.Configurations.Add(new TransactionMap());
            modelBuilder.Configurations.Add(new TransactionTypeMap());
            modelBuilder.Configurations.Add(new UserProfileMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new webpages_MembershipMap());
            modelBuilder.Configurations.Add(new webpages_OAuthMembershipMap());
            modelBuilder.Configurations.Add(new webpages_RolesMap());
            modelBuilder.Configurations.Add(new webpages_UsersInRolesMap());
            modelBuilder.Configurations.Add(new vw_TransactionMap());
            modelBuilder.Configurations.Add(new vw_VouchersMap());
            modelBuilder.Configurations.Add(new ServiceItemMap());
            modelBuilder.Configurations.Add(new WPItemMap());
            modelBuilder.Entity<WPItem>().Property(p => p.Rate).HasPrecision(18, 4);

            modelBuilder.Entity<UserCompany>()
                .HasRequired(uc => uc.Company) 
                .WithMany()
                .HasForeignKey(uc => uc.AuthCompanyId);



        }

        //public override int SaveChanges()
        //{
        //    foreach (var auditableEntity in ChangeTracker.Entries<BaseEntity>())
        //    {
        //        if (auditableEntity.State == EntityState.Added ||
        //            auditableEntity.State == EntityState.Modified)
        //        {
        //            // implementation may change based on the useage scenario, this
        //            // sample is for forma authentication.

        //            // modify updated date and updated by column for 
        //            // adds of updates.
        //            auditableEntity.Entity.LastModified = DateTime.Now;
        //            //auditableEntity.Entity.LastModifiedBy  = SiteContext

        //            // pupulate created date and created by columns for
        //            // newly added record.
        //            if (auditableEntity.State == EntityState.Added)
        //            {
        //                auditableEntity.Entity.Created = DateTime.Now;
        //                auditableEntity.Entity.CreatedBy = 1;
        //            }
        //            else
        //            {
        //                // we also want to make sure that code is not inadvertly
        //                // modifying created date and created by columns 
        //                auditableEntity.Property(p => p.Created).IsModified = false;
        //                auditableEntity.Property(p => p.CreatedBy).IsModified = false;
        //            }
        //        }
        //    }
        //    return 1;
        //}


    }
}
