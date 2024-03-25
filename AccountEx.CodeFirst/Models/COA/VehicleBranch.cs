using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace Entities.CodeFirst
{

    public partial class VehicleBranch : BaseEntity
    {
    //    public Vehicle() {
    //        this.VehicleFiles = new HashSet<VehicleFile>();
    //}
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Nullable<int> PostalCode { get; set; }
        public bool IsHeadOffice { get; set; }
        

        //public virtual ICollection<VehicleFile> VehicleFiles { get; set; }

    }
}
