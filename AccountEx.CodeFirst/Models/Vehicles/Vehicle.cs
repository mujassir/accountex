using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Models.Vehicles
{

    public partial class Vehicle : BaseEntity
    {
        public Vehicle()
        {
            this.VehicleFiles = new HashSet<VehicleFile>();
        }
        public Nullable<int> VendorId { get; set; }
        public int FileNo { get; set; }
        public int ManufacturerId { get; set; }
        public int ColorId { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public int EnginePowerId { get; set; }
        public string DoM { get; set; }
        public int ModelId { get; set; }
        public string RegNo { get; set; }
        public int FuelId { get; set; }
        public int CarTypeId { get; set; }
        public Nullable<int> YearId { get; set; }
        public Nullable<int> MonthId { get; set; }
        public Nullable<int> ClearingCompanyId { get; set; }
        public string EntryNo { get; set; }

        public Nullable<int> BranchId { get; set; }
        public bool IsSale { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> AssignedBranchId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ForexPrice { get; set; }
        public decimal ExcRate { get; set; }
        public bool IsForex { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public decimal SalePrice { get; set; }
        public bool LogBookApplied { get; set; }
       
        public Nullable<System.DateTime> LogBookAppliedDate { get; set; }
        public Nullable<int> LogBookAppliedBy { get; set; }
        public bool LogBookReceived { get; set; }
        public Nullable<System.DateTime> LogBookReceivedDate { get; set; }
        public Nullable<int> LogBookReceivedBy { get; set; }
        public bool LogBookTransferred { get; set; }
        public Nullable<System.DateTime> LogBookTransferredDate { get; set; }
        public Nullable<int> LogBookTransferredBy { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public string ReturnRemarks { get; set; }

        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public virtual ICollection<VehicleFile> VehicleFiles { get; set; }


    }
}
