using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AccountEx.Repositories
{
    public class vw_RentAgreementsRepository : GenericRepository<vw_RentAgreements>
    {
        public vw_RentAgreementsRepository() : base() { }
        public vw_RentAgreementsRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public vw_RentAgreements GetByAccountIdShopId(int accountId,int shopId) 
        {
            return Db.vw_RentAgreements.Where(p => p.TenantAccountId == accountId && p.ShopId == shopId).FirstOrDefault();
        }



    }
}
