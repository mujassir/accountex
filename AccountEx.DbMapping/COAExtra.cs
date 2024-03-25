using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class CoaExtra
    {
    }
    public class LeafAccountExtra : Account
    {
        public List<AccountAttribute> AccountAttributes { get; set; }
        //public LeafAccountExtra()
        //{
        //    AccountAttributes=new List<AccountAttribute>
        //}
    }
}
