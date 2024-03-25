using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst
{
    public class VehicleRequest : BaseEntity
    {
        public int VehicleId { get; set; }
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public string Remarks { get; set; }
        public string Reason { get; set; }
    }
}
