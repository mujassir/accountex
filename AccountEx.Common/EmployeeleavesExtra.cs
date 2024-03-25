using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class EmployeeleavesExtra
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public int LeaveTypeId { get; set; }
        public int NumberOfLeaves { get; set; }
        public string Remarks { get; set; }
    }
}
