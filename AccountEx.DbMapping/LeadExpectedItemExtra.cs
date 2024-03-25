using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class LeadExpectedItemExtra
    {
        public LeadExpectedItemExtra()
        {

            LeadExpectedItems = new List<LeadExpectedItem>();
          
        }
        public List<LeadExpectedItem> LeadExpectedItems { get; set; }
    }
}
