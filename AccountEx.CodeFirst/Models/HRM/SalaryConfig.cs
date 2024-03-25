using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst
{
    public partial class SalaryConfig : BaseEntity
    {
        public SalaryConfig()
        {
            this.SalaryConfigItems = new HashSet<SalaryConfigItem>();
        }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Name { get; set; }
        public virtual ICollection<SalaryConfigItem> SalaryConfigItems { get; set; }
    }
}
