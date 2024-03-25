using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{

    public partial class PrintRepossessionLetter
    {
        public string AuctioneerName { get; set; }
        public string AuctioneerPoBoxNo { get; set; }
        public string AuctioneerAddress { get; set; }
        public string AuctioneerContactNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPoBoxNo { get; set; }
        public string CustomerContactNumber { get; set; }
        public string CustomerRoute { get; set; }
        public string RegNo { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int AdvertisementDays { get; set; }
        public int NotificationDays { get; set; }
        public decimal OutStandingAmount { get; set; }
        public Nullable<decimal> Charges { get; set; }
    }
}
