namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ItemGroupItem : BaseEntity
    {
        public int ItemGroupId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public decimal SaleLessRate { get; set; }
    }
}
