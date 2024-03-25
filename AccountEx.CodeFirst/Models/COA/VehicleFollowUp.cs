using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public class VehicleFollowUp : BaseEntity
    {
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
        public Nullable<DateTime> NextFollowUp { get; set; }
    }
}
