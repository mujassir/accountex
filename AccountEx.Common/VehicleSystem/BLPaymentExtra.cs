using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{

    public partial class BLPaymentExtra
    {
        public int Id { get; set; }
        public string BLNumber { get; set; }
        public int BLId { get; set; }
        public decimal Amount { get; set; }
        public int ChargeId { get; set; }
        public string InvoiceNo { get; set; }
        public string Charge { get; set; }
    }
    public partial class BLDetailForPayment
    {
        public int Id { get; set; }
        public string BLNumber { get; set; }
        public int TotalUnit { get; set; }

    }
}
