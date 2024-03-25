using System;
using System.Collections.Generic;


namespace AccountEx.CodeFirst.Models.Lab
{

    public partial class TestParameter : BaseEntity
    {
        public int TestId { get; set; }
        public decimal Price { get; set; }
        public int ParameterId { get; set; }
    }
}
