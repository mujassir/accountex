using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{
    public class GetVehicleCustomerBalances
    {
        public DateTime Date { get; set; }
        public string Customer { get; set; }
        public string LocalId { get; set; }
        public string ContactNumber { get; set; }
        public string RegNo { get; set; }
        public string ChassisNo { get; set; }
        public string ProductName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Deposit { get; set; }
        public decimal Balance { get; set; }
        public string SaleType { get; set; }

    }
}
