using System;
using System.Collections.Generic;
namespace AccountEx.CodeFirst.Models
{
    public partial class ESalary : BaseEntity
    {
        public ESalary()
        {
            SalaryItems = new HashSet<SalaryItem>();
        }
        public int VoucherNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }

        public virtual ICollection<SalaryItem> SalaryItems { get; set; }
    }
}