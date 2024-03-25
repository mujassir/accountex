using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class ServiceItemMap : EntityTypeConfiguration<ServiceItem>
    {
        public ServiceItemMap()
        {
            // Primary Key
            Property(t => t.Quantity).HasPrecision(18,4);

          
        }
    }
}
