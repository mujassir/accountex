using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class RentDetailExtra
    {
        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal RentArrears { get; set; }
        public decimal UCPercentArears { get; set; }
        public decimal ElectricityArrears { get; set; }
        public decimal SurCharge { get; set; }
    }
}
