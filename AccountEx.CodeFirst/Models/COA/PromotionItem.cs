namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class PromotionItem : BaseEntity
    {
        public int PromotionId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public decimal PromotionRatePurchase { get; set; }
        public decimal PromotionRateSale { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> CustomerGroupId { get; set; }

    }
}