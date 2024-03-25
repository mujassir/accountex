using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class AccountAttributeRepository : GenericRepository<AccountAttribute>
    {
        public List<AccountAttribute> GetByAccountId(int accountId)
        {
            return Collection.Where(p => p.AccountId == accountId).ToList();
        }
    }
}
