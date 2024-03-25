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
using AccountEx.CodeFirst.Models.Nexus;

namespace AccountEx.CodeFirst.Context
{
    /// <summary>
    /// Partial class to extend AccountExContext, to add Nexus DB tables
    /// </summary>
    public partial class AccountExContext : DbContext
    {
        public DbSet<Nexus_City> Nexus_City { get; set; }
        public DbSet<Nexus_Country> Nexus_Country { get; set; }
        public DbSet<Nexus_Case> Nexus_Case { get; set; }
        public DbSet<Nexus_CaseDetail> Nexus_CaseDetail { get; set; }
        public DbSet<Nexus_Consultant> Nexus_Consultant { get; set; }
        public DbSet<Nexus_Patient> Nexus_Patient { get; set; }
        public DbSet<Nexus_RateType> Nexus_RateType { get; set; }
        public DbSet<Nexus_Reference> Nexus_Reference { get; set; }
        public DbSet<Nexus_TestDepartment> Nexus_TestDepartment { get; set; }
        public DbSet<Nexus_TestGroup> Nexus_TestGroup { get; set; }
        public DbSet<Nexus_TestNormalValues> Nexus_TestNormalValues { get; set; }
        public DbSet<Nexus_Test> Nexus_Test { get; set; }
        public DbSet<Nexus_TestDoctor> Nexus_TestDoctor { get; set; }
        public DbSet<Nexus_TestSpecimen> Nexus_TestSpecimen { get; set; }
        public DbSet<NexusPostedCases> NexusPostedCases { get; set; }
        public DbSet<NexusPostedCasesItems> NexusPostedCasesItems { get; set; }
        public DbSet<DepartmentTest> DepartmentTests { get; set; }
        public DbSet<DepartmentRateList> DepartmentRateLists { get; set; }
        public DbSet<DepartmentRateListItem> DepartmentRateListItems { get; set; }
        public DbSet<NexusDepartmentMapping> NexusDepartmentMappings { get; set; }
        public DbSet<vw_NexusDepartmentMapping> vw_NexusDepartmentMapping { get; set; }
        public DbSet<Nexus_Vw_Cases> Nexus_Vw_Cases { get; set; }




    }
}
