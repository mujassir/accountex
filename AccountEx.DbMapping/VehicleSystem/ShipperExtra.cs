using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.DbMapping.VehicleSystem
{

    public partial class ShipperExtra
    {
        public virtual AccountDetail Shipper { get; set; }
        public virtual ICollection<SupplierCurrency> SupplierCurrencies { get; set; }
    }
}
