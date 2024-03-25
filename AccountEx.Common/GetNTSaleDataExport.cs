using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetNTSaleDataExport
    {
        public string FirmName { get; set; }
        public string PartyName { get; set; }
        public string Month { get; set; }
        public string DCDate { get; set; }
        public string Description { get; set; }
        public string VehicleNo { get; set; }
        public string DeliverTo { get; set; }
        public int GatePassNumber { get; set; }
        public decimal GSM { get; set; }
        public decimal Width { get; set; }
        public decimal TotalMeters { get; set; }
        public decimal Kgs { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal Value { get; set; }
    }
}
