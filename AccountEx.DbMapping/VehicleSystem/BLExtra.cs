using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.DbMapping.VehicleSystem
{
    public class BLExtra
    {
        public BL BL { get; set; }
        public List<BLItemExtra> BLItemExtra { get; set; }


    }
    public class BLItemExtra : Vehicle
    {
        public int VehicleId { get; set; }
        public int BLId { get; set; }


    }
}
