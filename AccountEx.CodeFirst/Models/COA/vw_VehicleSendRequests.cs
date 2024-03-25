using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst
{
    public class vw_VehicleSendRequests
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string OwnerBranch { get; set; }
        public string Manufacturer { get; set; }
        public int OwnerBranchId { get; set; }
        public string ReceivingBranch { get; set; }
        public int ReceivingBranchId { get; set; }
        public string DoM { get; set; }
        public string ChassisNo { get; set; }
        public string Model { get; set; }
        public string RegNo { get; set; }
        public string EnginePower { get; set; }
        public string Fuel { get; set; }
        public string EngineNo { get; set; }
        public string Color { get; set; }
        public byte Status { get; set; }
        public byte Type { get; set; }
        public string RequestType { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
