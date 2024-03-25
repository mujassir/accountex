using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetSalePurchaseDateWiseSummaryByVoucherTypeExtra
    {

        public DateTime Date { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GstAmountTotal { get; set; }
        public decimal NetTotal { get; set; }

    }
}
