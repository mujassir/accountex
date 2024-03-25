using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public partial class GetCustomerCollections
    {
        public System.DateTime Date { get; set; }
        public string Customer { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string ProductName { get; set; }
        public string Year { get; set; }
        public string PaymentAccount { get; set; }
        public decimal Amount { get; set; }
    }
}
