using System.Linq;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class WheatPurchaseItemRepository : GenericRepository<WheatPurchaseItem>
    {

        public int GetNextBillNo(bool isGOVT)
        {
            const int maxnumber = 1;
            if (!AsQueryable<WheatPurchase>(true).Where(p=>p.IsGovt==isGOVT).Any(p => p.WheatPurchaseItems.Any()))
                return maxnumber;
            var voucher = AsQueryable<WheatPurchase>(true).Where(p => p.IsGovt == isGOVT).SelectMany(p => p.WheatPurchaseItems).OrderByDescending(p => p.BillNo).FirstOrDefault();
            if (voucher != null)
                return voucher.BillNo + 1;
            return maxnumber;
        }



        //public double? GetTodaySale(int accountId, DateTime date)
        //{
        //    return Collection.Where(p => p.AccountId == accountId && p.Date == date).Sum(p => (double?)p.Quantity * (double?)p.Price);
        //}

        public WheatPurchaseItem CheckIfBillNoExist(List<int> billnso, List<int> ids, bool isGOVT)
        {
            return AsQueryable<WheatPurchase>(true).Where(p => p.IsGovt == isGOVT).SelectMany(p => p.WheatPurchaseItems).FirstOrDefault(p => billnso.Contains(p.BillNo) && !ids.Contains(p.Id));
        }
        public bool CheckIfSalePurchaseExist(List<int> accounts)
        {
            return AsQueryable<WheatPurchase>(true).SelectMany(p => p.WheatPurchaseItems).Any(p => accounts.Contains(p.ItemId));
        }
    }
}