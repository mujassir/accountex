using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.Views
{
    public class vw_VehicleSales
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int VoucherNumber { get; set; }
        public System.DateTime Date { get; set; }
        public string Customer { get; set; }
        public string ContactNumber { get; set; }
        public string LocalId { get; set; }
        public string Address { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string CarType { get; set; }
        public string EngineNo { get; set; }
        public string Color { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string EnginePower { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Received { get; set; }
        public decimal Balance { get; set; }
        public VoucherType SaleType { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFinal { get; set; }
        public bool IsDelivered { get; set; }
        public byte RecoveryStatus { get; set; }
        public Nullable<System.DateTime> DeliveredDate { get; set; }
        public Nullable<int> DeliveredBy { get; set; }
        public string DeliveredByName { get; set; }
        public string DeliveredRemarks { get; set; }
        public string CreateByName { get; set; }


        public bool IsVoid { get; set; }
    }
}
