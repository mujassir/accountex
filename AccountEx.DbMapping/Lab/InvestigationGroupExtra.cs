using AccountEx.CodeFirst.Models.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AccountEx.DbMapping.Lab
{
    public class InvestigationGroupExtra
    {
        public int?  GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Test> Investigations { get; set; }
    }
}
