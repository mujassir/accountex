using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class EmployeeOTHourExtra : EmployeeOTHour
    {
        public List<EmployeeOTHour> EmployeeOTHourList { get; set; }
        public string RecordType { get; set; }
    }
}
