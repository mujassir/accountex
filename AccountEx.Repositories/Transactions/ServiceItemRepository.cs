using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class ServiceItemRepository : GenericRepository<ServiceItem>
    {
        public ServiceItemRepository() : base() { }
        public ServiceItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public void Save(List<ServiceItem> items, int serviceid)
        {
            //string query = "Delete from ServiceItems where ServiceId=" + serviceid + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            base.Save(items);
            Db.SaveChanges();
        }
        public List<ServiceItem> GetByServiceId(int serviceId)
        {
            return Collection.Where(p => p.ServiceId == serviceId).ToList();
        }
        public void DeleteByServiceId(int serviceId)
        {
            string query = "Delete from ServiceItems where ServiceId=" + serviceId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }


    }
}
