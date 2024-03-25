using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.CodeFirst.Models.Lab
{
    public partial class TestCategory : BaseEntity
    {
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
    }
}
