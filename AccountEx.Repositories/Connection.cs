using System.Data.SqlClient;
using System.Configuration;
using AccountEx.CodeFirst.Context;

namespace AccountEx.Repositories
{
    public static class Connection
    {
        public static AccountExContext GetContext()
        {
            var db = new AccountExContext();
            return db;
        }
        public static AccountExContext GetContext1()
        {
            //var Application = SiteContext.Current.Application;
            var application = "Default";
            if (string.IsNullOrWhiteSpace(application)) application = "Default";
            var db = new AccountExContext(application);
            return db;
        }
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["AccountEx"].ConnectionString;
        }

        public static string GetNexusConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Nexus"].ConnectionString;
        }

        public static SqlConnection GetsqlConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
       
    }

}
