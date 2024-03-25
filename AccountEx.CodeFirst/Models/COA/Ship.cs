using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class Ship : BaseEntity
    {
        public int ShipperId { get; set; }
        public string Name { get; set; }
        public string VESSEL { get; set; }
    }
}
