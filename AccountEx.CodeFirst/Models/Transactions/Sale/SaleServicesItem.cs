using System;
namespace AccountEx.CodeFirst.Models
{
    public partial class SaleServicesItem : BaseEntity
    {

        public int SaleId { get; set; }
        public Nullable<int> SaleItemId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> ServiceItemId { get; set; }
        public int ServiceItemItemId { get; set; }
        public string ServiceItemName { get; set; }
        public string ServiceItemCode { get; set; } 
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        
    }
}