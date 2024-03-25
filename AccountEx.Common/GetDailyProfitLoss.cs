using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetDailyProfitLoss
    {
        public Nullable<DateTime> SaleDate { get; set; }
        public string Item { get; set; }
        public decimal NetAmount { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> Expense { get; set; }
        public Nullable<decimal> GrossProfit { get; set; }

    }
}
