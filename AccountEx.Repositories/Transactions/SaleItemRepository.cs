using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class SaleItemRepository : GenericRepository<SaleItem>
    {
        public SaleItemRepository() : base() { }
        public SaleItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public int GetNextSaleTaxNo()
        {
            const int maxnumber = 1;
            if (!AsQueryable<Sale>(true).Any(p => p.SaleItems.Any()))
                return maxnumber;
            var voucher = AsQueryable<Sale>(true).SelectMany(p => p.SaleItems).OrderByDescending(p => p.SaleTaxNo).FirstOrDefault();
            if (voucher != null)
                return voucher.SaleTaxNo + 1;
            return maxnumber;
        }
        public bool CheckIfSalePurchaseExist(List<int> accounts)
        {
            return Collection.Any(p => accounts.Contains(p.ItemId));
        }
        public SaleItem CheckIfTaxNoExist(List<int> taxnos, List<int> ids)
        {
            return AsQueryable<Sale>(true).SelectMany(p => p.SaleItems).FirstOrDefault(p => taxnos.Contains(p.SaleTaxNo) && !ids.Contains(p.Id));
        }
        public SaleItem GetBySaleId(int saleid)
        {
            return Collection.Where(p=>p.SaleId==saleid).AsNoTracking().FirstOrDefault();
        }
      
    }
}