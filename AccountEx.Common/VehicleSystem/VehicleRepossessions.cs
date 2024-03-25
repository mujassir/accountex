using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{
    public class VehicleRepossessions
    {
        public Nullable<System.DateTime> LetterDate { get; set; }
        public string Customer { get; set; }
        public string ContactNumber { get; set; }
        public string LocalId { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string Auctionner { get; set; }
        public string LetterIssued { get; set; }
        public string Repossessed { get; set; }
        public Nullable<System.DateTime> RepossessedDate { get; set; }
        public string PendingRepossessed { get; set; }
        public int NotificationDays { get; set; }
        public Nullable<System.DateTime> AdvertisementDate { get; set; }
        public string Newspaper { get; set; }
        public string AgreementRemarks { get; set; }
        public string InventoryReturn { get; set; }
        public string Settlement { get; set; }
        public string Resold { get; set; }
        public byte RecoveryStatus { get; set; }
    }
}
