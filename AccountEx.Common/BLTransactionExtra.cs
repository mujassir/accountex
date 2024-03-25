using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class BLTransactionExtra
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string ChassisNo { get; set; }
        public string EngineNo { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsDeleted { get; set; }
       


    }
}
