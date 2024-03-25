using System;

namespace AccountEx.CodeFirst.Models
{
     [Serializable]
    public partial class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
