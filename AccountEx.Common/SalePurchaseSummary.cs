using System;

namespace AccountEx.Common
{
    public class YearlySalePurchaseSummary
    {
        public string Year { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal NetTotal { get; set; }
    }

    public class YearlyComparison
    {


        public string Year { get; set; }
        public decimal Sales { get; set; }
        public decimal CGS { get; set; }
        public decimal Expeneses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal GDP { get; set; }
        public decimal Percent { get; set; }
    }
}
