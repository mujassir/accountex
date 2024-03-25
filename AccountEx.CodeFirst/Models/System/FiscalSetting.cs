using System;

namespace AccountEx.CodeFirst.Models
{
     [Serializable]
    public partial class FiscalSetting : BaseEntity
    {
        public int FiscalId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
