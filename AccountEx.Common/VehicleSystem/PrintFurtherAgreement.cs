using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{
    public partial class PrintFurtherAgreement
    {
        public string CustomerName { get; set; }
        public string LocalId { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string AgreementRemarks { get; set; }
        public decimal OutStandingAmount { get; set; }
    }
}
