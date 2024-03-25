using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class SalaryConfigItemRepository : GenericRepository<SalaryConfigItem>
    {
        public SalaryConfigItemRepository() : base() { }
        public SalaryConfigItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public SalaryConfigItem GetSalaryConfigBydate(int accointId, int salaryConfigId)
        {
            var salaryItem=Collection.FirstOrDefault(p => p.AccountId == accointId && p.SalaryConfigId == salaryConfigId);
            return salaryItem;
        }
    }
}
