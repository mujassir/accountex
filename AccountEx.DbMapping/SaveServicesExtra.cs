using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.DbMapping
{
    public class SaveServicesExtra
    {
        public SaveServicesExtra()
        {
           
            ServiceItems = new List<ServiceItem>();
           
        }
        public AccountDetail Service { get; set; }
        public List<ServiceItem> ServiceItems { get; set; }
       
    }
}
