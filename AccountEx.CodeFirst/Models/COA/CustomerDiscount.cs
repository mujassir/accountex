using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class CustomerDiscount : BaseEntity
    {
        
        public Nullable<int> CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductTitle { get; set; }
        public Nullable<int> COAProductId { get; set; }
        public string ProductCode { get; set; }
        public decimal Discount { get; set; }
        public int FiscalId { get; set; }
        
    }
}
