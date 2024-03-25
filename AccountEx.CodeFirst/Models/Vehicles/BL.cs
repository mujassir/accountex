using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class BL : BaseEntity
    {
        public BL()
        {
            BLItems = new HashSet<BLItem>();
            BLCharges= new HashSet<BLCharge>();
        }

        public int VoucherNumber { get; set; }
        public string BLNumber { get; set; }
        public Nullable<int> ShipperId { get; set; }
        public int SupplierId { get; set; }
        public Nullable<int> ConsigneeId { get; set; }
        public int TotalUnits { get; set; }
        public int AddedUnits { get; set; }
        public int RemainingUnits { get; set; }
        
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Address { get; set; }
        public Nullable<int> ShipId { get; set; }
        public string ShipName { get; set; }
        public Nullable<int> Units { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> ArrivalDate { get; set; }
        public Nullable<DateTime> DepartureDate { get; set; }
        public Nullable<DateTime> DateOfLoading { get; set; }
        public decimal DeliveryOrder { get; set; }
        public decimal PortCharges { get; set; }
        public decimal Fare { get; set; }
        public decimal Storage { get; set; }
        public string PortOfLoading { get; set; }
        public string PlaceOfReceipt { get; set; }
        public string PlaceOfDelivery { get; set; }
        public string PlaceOfIssue { get; set; }
        public Nullable<DateTime> DateOfIssue { get; set; }
        public string Remarks { get; set; }
        public int StatusId { get; set; }
        public bool IsFinal { get; set; }
        public virtual ICollection<BLItem> BLItems { get; set; }
        public virtual ICollection<BLCharge> BLCharges { get; set; }
    }
}
