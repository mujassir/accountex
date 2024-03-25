using System.Collections.Generic;
using AccountEx.CodeFirst.Models;
namespace AccountEx.DbMapping
{
    public class EmployeeLeaveExtra : EmployeeLeave
    {
        public List<EmployeeLeave> EmployeeLeaveList { get; set; }
        public string RecordType { get; set; }
    }
}
