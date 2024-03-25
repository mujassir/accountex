using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{

    public partial class VehicleIncomeStatment
    {

        public int NoOfUnits { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Cost { get; set; }
        public decimal IndriectExpenses { get; set; }
    }
}
