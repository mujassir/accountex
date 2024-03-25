using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public class ProductionDetail
    {

     
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public decimal NetTotal { get; set; }
        public decimal FinishedNetTotal { get; set; }
        public System.DateTime Date { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal Difference { get; set; }


    }
}
