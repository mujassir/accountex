using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Models.Vehicles
{

    public partial class VehicleFile : BaseEntity
    {
        public Nullable<int> VehicleId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }
    }
}
