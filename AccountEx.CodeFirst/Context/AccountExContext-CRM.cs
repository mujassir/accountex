using System.Data.Entity;
using AccountEx.CodeFirst.Models.CRM;





namespace AccountEx.CodeFirst.Context
{
    public partial class AccountExContext : DbContext
    {
        public DbSet<Division> Divisions { get; set; }
        public DbSet<UOM> UOMS { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSecCategory> ProductSecCategories { get; set; }
        public DbSet<ProductSubGroup> ProductSubGroups { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<ProductLostReason> ProductLostReasons { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<vw_CRMUser> vw_CRMUsers { get; set; }
        public DbSet<CRMProduct> CRMProducts { get; set; }
        public DbSet<vw_CRMPRoducts> vw_CRMProducts { get; set; }
        public DbSet<CRMCustomer> CRMCustomers { get; set; }
        public DbSet<CRMVendor> CRMVendors { get; set; }
        public DbSet<PMCItem> PMCItems { get; set; }
        public DbSet<PMC> PMC { get; set; }
        public DbSet<CRMProject> CRMProjects { get; set; }
        public DbSet<vw_CRMProjects> vw_CRMProjects { get; set; }
        public DbSet<vw_CRMPMCItems> vw_CRMPMCItems { get; set; }

        public DbSet<vw_CRMCustomers> vw_CRMCustomers { get; set; }
        public DbSet<vw_CRMPMCS> vw_CRMPMCS { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<CRMImportRequisition> CRMImportRequisitions { get; set; }
        public DbSet<CRMImportRequisitionItem> ImportRequisitionItems { get; set; }
        public DbSet<CRMSaleInvoice> CRMSales { get; set; }
        public DbSet<vw_CRMSaleInvoices> vw_CRMSaleInvoices { get; set; }
        public DbSet<CRMSaleInvoiceItem> CRMSaleInvoiceItems { get; set; }

        public DbSet<EventMode> EventModes { get; set; }
        public DbSet<EventStatus> EventStatuses { get; set; }

        public DbSet<SaleType> SaleTypes { get; set; }
        public DbSet<CRMCustomerSalePerson> CRMCustomerSalePersons { get; set; }
        public DbSet<vw_CRMImportRequisition> vw_CRMImportRequisitions { get; set; }
        public DbSet<CRMSaleForecast> CRMSaleForecasts { get; set; }
        public DbSet<CRMComplaint> CRMComplaint { get; set; }
        public DbSet<ClusterType> ClusterTypes { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<CRMCaseType> CRMCaseType { get; set; }
        public DbSet<ComplaintStatus> ComplaintStatuses { get; set; }
        public DbSet<ComplaintTest> ComplaintTests { get; set; }
        public DbSet<ComplaintLab> ComplaintLabs { get; set; }
        public DbSet<vw_CRMCompliants> vw_CRMCompliants { get; set; }
        public DbSet<vw_Divisions> vw_Divisions { get; set; }
        public DbSet<vw_CRMSaleInvoiceItems> vw_CRMSaleInvoiceItems { get; set; }
        public DbSet<vw_ProductSubGroups> vw_ProductSubGroups { get; set; }
        public DbSet<vw_CRMCalendarEvents> vw_CRMCalendarEvents { get; set; }
        public DbSet<CRMOwnershipTransfer> CRMOwnershipTransfers { get; set; }
        public DbSet<CRMImportGRN> CRMImportGRNS { get; set; }
        public DbSet<vw_CRMImportIGRN> vw_CRMImportIGRN { get; set; }

    }
}
