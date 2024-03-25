using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class RoleActionRepository : GenericRepository<RoleAction>
    {
         public RoleActionRepository() : base() { }
         public RoleActionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<RoleAction> GetByRoleId(int roleId)
        {
            return Collection.Where(p => p.RoleId == roleId).ToList();
        }
        public List<RoleAction> GetByCompanyId(int companyId)
        {
            return Collection.Where(p => p.CompanyId == companyId).ToList();
        }
        public List<RoleAction> GetByRoleId(List<int> roleIds)
        {
            return Collection.Where(p => roleIds.Contains(p.RoleId)).ToList();
        }
    }
}
