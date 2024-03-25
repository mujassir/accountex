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
using AccountEx.CodeFirst.Models.Lab;

namespace AccountEx.CodeFirst.Context
{
    /// <summary>
    /// Partial class to extend AccountExContext, to add Nexus DB tables
    /// </summary>
    public partial class AccountExContext : DbContext
    {

        public DbSet<TestCategory> TestGroups { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestParameter> TestParameters { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<vw_Investigations> vw_Investigations { get; set; }



    }
}
