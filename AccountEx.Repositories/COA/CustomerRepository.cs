using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>
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
        public override List<IdName> GetNames()
        {
            return Collection.Select(p => new { p.Id, p.Code, p.Name }).ToList().Select(p => new IdName { Id = p.Id, Name = p.Code + "-" + p.Name }).ToList();
        }
    }
}
