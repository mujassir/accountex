namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;

    public class GroupItemCustomerLess : BaseEntity
    {
        public int CustomerGroupId { get; set; }
        public int ItemGroupId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public decimal LessRate { get; set; }

    }
}