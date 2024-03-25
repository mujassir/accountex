using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{
    public class AccountTypeExtra : AccountType
    {
        public List<AccountAttribute> Attributes { get; set; }
    }
}
