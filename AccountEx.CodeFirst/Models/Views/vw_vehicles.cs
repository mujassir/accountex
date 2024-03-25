using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Views
{
    public class vw_Vehicles
    {
        public int Id { get; set; }
        public int FileNo { get; set; }
        public string VehicleName { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public string EnginePower { get; set; }
        public string DoM { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string RegNo { get; set; }
        public string Fuel { get; set; }
        public string CarType { get; set; }
        public string ClearingCompany { get; set; }
        public string EntryNo { get; set; }
        public string LocalVendor { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public bool IsSale { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> AssignedBranchId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        public byte Type { get; set; }
        public Nullable<int> VendorId { get; set; }
        public bool LogBookApplied { get; set; }
        public Nullable<System.DateTime> LogBookAppliedDate { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<int> LogBookAppliedBy { get; set; }
        public bool LogBookReceived { get; set; }
        public Nullable<System.DateTime> LogBookReceivedDate { get; set; }
        public Nullable<int> LogBookReceivedBy { get; set; }
        public bool LogBookTransferred { get; set; }
        public Nullable<System.DateTime> LogBookTransferredDate { get; set; }
        public Nullable<int> LogBookTransferredBy { get; set; }
        public byte Status { get; set; }
    }
}
