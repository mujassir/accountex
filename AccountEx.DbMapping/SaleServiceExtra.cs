using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class SaleServiceExtra
    {
        public SaleServiceExtra()
        {
            Sale = new Sale();
            SaleItems = new List<SaleItem>();
            SaleServicesItems = new List<SaleServicesItem>();
        }
        public Sale Sale { get; set; }
        public List<SaleItem> SaleItems { get; set; }
        public List<SaleServicesItem> SaleServicesItems { get; set; }
    }
}
