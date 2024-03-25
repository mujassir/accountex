using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class BLItem : BaseEntity
    {
        public int BLId { get; set; }
        public int VehicleId { get; set; }
        public string ChassisNo { get; set; }
    }
}
