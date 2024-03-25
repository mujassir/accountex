using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class TenantPartnerRepository : GenericRepository<TenantPartner>
    {
        public override void Save(List<TenantPartner> entities)
        {
            var query = "Delete from TenantPartners where TenantId=" + entities.FirstOrDefault().TenantId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            base.Save(true, entities);
        }
        public List<TenantPartner> GetByTenantId(int tenantid)
        {
            return Collection.Where(p => p.TenantId == tenantid).ToList();

        }
        public void DeleteByTenantId(int tenantid)
        {
            var query = "Delete from TenantPartners where TenantId=" + tenantid + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
        }
    }
}
