using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class TenantExtra
    {
        public TenantExtra()
        {
            AccountDetail = new AccountDetail();
            TenantPartners = new List<TenantPartner>();
        }

        public AccountDetail AccountDetail { get; set; }
        public List<TenantPartner> TenantPartners { get; set; }
    }
   
}
