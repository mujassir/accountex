using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class ProductionItemRepository : GenericRepository<WPItem>
    {




        //public double? GetTodaySale(int accountId, DateTime date)
        //{
        //    return Collection.Where(p => p.AccountId == accountId && p.Date == date).Sum(p => (double?)p.Quantity * (double?)p.Price);
        //}


        public bool CheckIfProductionExist(List<int> accounts)
        {
            return Collection.Any(p => accounts.Contains(p.ItemId));
        }
    }
}