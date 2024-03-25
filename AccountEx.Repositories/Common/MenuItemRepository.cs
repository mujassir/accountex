using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>
    {
        public List<MenuItem> GetByCompanyId(int companyId)
        {
            return Collection.Where(p => p.CompanyId == companyId).ToList();
        }

        public override void Delete(int Id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var menu = Collection.FirstOrDefault(p => p.Id == Id);
                if (menu != null)
                {
                    new RoleAccessRepository().DeleteByMenuId(Id);
                    base.Delete(Id);
                }

                scope.Complete();
            }

        }
    }
}
