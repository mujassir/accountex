using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class Product : BaseEntity
    {
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Others { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> PackingPerCarton { get; set; }
    }
}
