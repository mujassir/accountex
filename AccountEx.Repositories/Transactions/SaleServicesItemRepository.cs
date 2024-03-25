using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class SaleServicesItemRepository : GenericRepository<SaleServicesItem>
    {
         public SaleServicesItemRepository() : base() { }
         public SaleServicesItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public void Save(List<SaleServicesItem> items, int serviceid)
        {
            string query = "Delete from SaleServicesItems where SaleId=" + serviceid + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            base.Add(items);
        }
        public List<SaleServicesItem> GetBySaleId(int saleId)
        {
            return Collection.Where(p => p.SaleId == saleId).ToList();
        }
        public void DeleteBySaleId(int saleId)
        {
            string query = "Delete from SaleServicesItems where SaleId=" + saleId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }
        public bool IsItemUsed(int id)
        {
            return Collection.Any(p => p.ServiceItemId == id);
        }


    }
}
