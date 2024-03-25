using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class LeadConcernedItemExtra
    {
        public LeadConcernedItemExtra()
        {

            LeadConcernedItems = new List<LeadConcernedItem>();
          
        }
        public List<LeadConcernedItem> LeadConcernedItems { get; set; }
    }
}
