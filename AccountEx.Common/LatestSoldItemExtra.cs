using System.Collections.Generic;

namespace AccountEx.Common
{
    public class LatestSoldItemExtra
    {
        public int AccountId { get; set; }
        public int ItemId { get; set; }
        public decimal BardanaRate { get; set; }
        public decimal WheatRate { get; set; }
       

    }

    public class LatestSaleRate
    {
        public int AccountId { get; set; }
        public int ItemId { get; set; }
        public decimal Rate { get; set; }
      

    }
  

}
