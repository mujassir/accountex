using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{

    public partial class SupplierCurrency : BaseEntity
    {
        public int AccountId { get; set; }
        public int CurrencyId { get; set; }
    }
}
