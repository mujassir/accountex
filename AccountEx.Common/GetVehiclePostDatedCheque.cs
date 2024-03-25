using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public class GetVehiclePostDatedCheque
    {
        public string ChequeNo { get; set; }
        public System.DateTime ChequeDate { get; set; }
        public Nullable<int> VehicleSaleId { get; set; }
        public string RegNo { get; set; }
        public string ChassisNo { get; set; }
        public decimal Amount { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string Banks { get; set; }
        public string Customer { get; set; }
    }
}
