using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class PurchaseTotalItemRepository : GenericRepository<PurchaseTotalItem>
    {


        public List<PurchaseTotalItem> GetByPurchaseId(int id)
        {
            return Collection.Where(p => p.PurchaseId == id).ToList();
        }

    }
}