using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class WheatPurchaseExtra
    {
        public WheatPurchaseExtra()
        {
            WheatPurchases = new List<WheatPurchase>();
           
        }
        
        public List<WheatPurchase> WheatPurchases { get; set; }
       
    }
}
