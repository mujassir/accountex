using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class ActionRepository : GenericRepository<Action>
    {
        public List<Action> GetByCompanyId(int companyId)
        {
            return Collection.Where(p => p.CompanyId == companyId).ToList();
        }
    }
}
