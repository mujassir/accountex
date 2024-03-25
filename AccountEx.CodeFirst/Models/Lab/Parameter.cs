using System;
using System.Collections.Generic;


namespace AccountEx.CodeFirst.Models.Lab
{
    public partial class Parameter : BaseEntity
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public byte Type { get; set; }
        public Nullable<int> TestGroupId { get; set; }
        public string NormalValuesComment { get; set; }
    }
}
