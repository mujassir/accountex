using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class BLStatusHistory : BaseEntity
    {
        public int VehicleId { get; set; }
        public int Status { get; set; }
        public DateTime FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }

    }
}
