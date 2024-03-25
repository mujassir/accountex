using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class ProductMapping : BaseEntity
    {
        
        public Nullable<int> CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductTitle { get; set; }
        public Nullable<int> COAProductId { get; set; }
        public string ProductCode { get; set; }
        public string ManualCode { get; set; }
        public int FiscalId { get; set; }
        
    }
}
