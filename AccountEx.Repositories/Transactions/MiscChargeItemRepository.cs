using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class MiscChargeItemRepository : GenericRepository<MiscChargeItem>
    {

        public MiscChargeItem GetByMiscChargeId(int miscid)
        {
            return Collection.FirstOrDefault(p => p.MiscChargeId == miscid);
        }
        public MiscChargeItem GetByTenantId(int tenantId)
        {
            return Collection.Where(p => p.TenantAccountId == tenantId).FirstOrDefault();
        }
    }
}