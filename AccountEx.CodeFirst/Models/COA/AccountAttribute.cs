using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class AccountAttribute : BaseEntity
    {
      
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}
