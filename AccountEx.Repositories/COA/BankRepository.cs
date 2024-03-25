using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class BankRepository : GenericRepository<Bank>
    {
        public void SyncIds(List<int> ids)
        {
            foreach (var account in AsQueryable<Account>().Where(p => ids.Contains(p.Id)).ToList())
            {
                var item = Collection.FirstOrDefault(p => p.AccountId == account.Id);
                if (item != null)
                    account.ReferenceId = item.Id;
            }
            Db.SaveChanges();
        }
    }
}
