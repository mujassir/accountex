using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class vw_BLS
    {


        public int Id { get; set; }
        public int VoucherNumber { get; set; }
        public string BLNumber { get; set; }
        public int SupplierId { get; set; }
        public int TotalUnits { get; set; }
        public int AddedUnits { get; set; }
        public string SupplierName { get; set; }
        public string ShipName { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public Nullable<DateTime> ArrivalDate { get; set; }
        public Nullable<DateTime> DepartureDate { get; set; }
        public bool IsFinal { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
       
    }
}
