using System.Data.Entity;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.Config;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;

namespace AccountEx.CodeFirst.Context
{
    public partial class AccountExContext : DbContext
    {
        public DbSet<PackagingType> PackagingTypes { get; set; }
        public DbSet<AccountAttribute> AccountAttributes { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public DbSet<Configuration> Configs { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<Marka> Markas { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionItem> PromotionItems { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<ItemGroupItem> ItemGroupItems { get; set; }
        public DbSet<GroupItemCustomerLess> GroupItemCustomerLesses { get; set; }
        public DbSet<CompanyPartner> CompanyPartners { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Generic> Generic { get; set; }
        public DbSet<Manufacture> Manufacture { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<EnginePower> EnginePowers { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Consignee> Consignees { get; set; }
        public DbSet<Shipeer> Shipeers { get; set; }
        public DbSet<ClearingCompany> ClearingCompanies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<MonthName> MonthNames { get; set; }
        public DbSet<Year> Years { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<WorkingSector> WorkingSectors { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<PortofLoading> PortofLoadings { get; set; }
        public DbSet<PortOfDischarge> PortOfDischarges { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<DeliveryTerm> DeliveryTerms { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
    }
}
