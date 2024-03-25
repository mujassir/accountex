using AccountEx.CodeFirst.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.DbMapping.VehicleSystem
{
    public class VehicleLogBookGet
    {

        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> LogBookAppliedDate { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public Nullable<DateTime> LogBookReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string SoldStatus { get; set; }
        public string ClearedBy { get; set; }
        public string TransfeeredToWhom { get; set; }
        public int Status { get; set; }
    }
    public class VehicleLogBookSave
    {

        public VehicleLogBookSave()
        {
            VehicleLogBookScanes = new List<VehicleLogBookScane>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public byte Type { get; set; }
        public List<VehicleLogBookScane> VehicleLogBookScanes { get; set; }
    }
}
