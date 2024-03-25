using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class WPItemMap : EntityTypeConfiguration<WPItem>
    {
        public WPItemMap()
        {
            // Primary Key
            Property(t => t.Quantity).HasPrecision(18, 4);


        }
    }
}
