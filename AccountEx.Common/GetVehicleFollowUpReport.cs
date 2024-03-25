using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AccountEx.Common
{
  public partial  class GetVehicleFollowUpReport
    {
        public string Customer { get; set; }
        public string ContactNumber { get; set; }
        public string ContactNumber1 { get; set; }
        public string ProductName { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> NextFollowUp { get; set; }
        public string Auctionner { get; set; }
        public Nullable<System.DateTime> LetterDate { get; set; }
        public System.DateTime Date { get; set; }
        public string Status { get; set; }
        public string NotificationDays { get; set; }
        public string AdvertisementDays { get; set; }
        public string LogBookStatus { get; set; }
    }
}
