using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetNTPurchaseDataExport
    {
        public string Month { get; set; }
        public string PartyName { get; set; }
        public string GatePassDate { get; set; }
        public int GatePassNumber { get; set; }
        public string VehicleNo { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
        public decimal NetAmount { get; set; }

    }
}
