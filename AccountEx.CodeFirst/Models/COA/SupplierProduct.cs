using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class SupplierProduct : BaseEntity
    {
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }

    }
}
