using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class StorePurchaseTotalItemRepository : GenericRepository<StorePurchaseTotalItem>
    {


        public List<StorePurchaseTotalItem> GetByPurchaseId(int id)
        {
            return Collection.Where(p => p.PurchaseId == id).ToList();
        }

    }
}