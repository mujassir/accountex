using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class RoleAccessRepository : GenericRepository<RoleAccess>
    {
         public RoleAccessRepository() : base() { }
         public RoleAccessRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<RoleAccess> GetByRoleId(int roleId)
        {
            return Collection.Where(p => p.RoleId == roleId).ToList();
        }
        public List<RoleAccess> GetCommonAccess(List<int> roleIds)
        {
            return Collection.Where(p => roleIds.Contains(p.RoleId)).GroupBy(p => p.MenuItemId).ToList().Select(p => new RoleAccess()
            {
                CanView = p.Any(q => q.CanView),
                CanUpdate = p.Any(q => q.CanUpdate),
                CanDelete = p.Any(q => q.CanDelete),
                CanCreate = p.Any(q => q.CanCreate),
                CanAuthorize = p.Any(q => q.CanAuthorize),
                MenuItemId = p.Key,
            }).ToList();
        }
        public void DeleteByMenuId(int menuId)
        {
            foreach (var item in Collection.Where(p => p.MenuItemId == menuId))
            {
                Db.RoleAccesses.Remove(item);
            }
            Db.SaveChanges();
        }
        public List<RoleAccess> GetByRoleId(List<int> roleIds)
        {
            return Collection.Where(p => roleIds.Contains(p.RoleId)).ToList();
        }
        public List<RoleAccess> GetByCompanyId(int companyId)
        {
            return Collection.Where(p => p.CompanyId == companyId).ToList();
        }
        public RoleAccess GetByMenuItemId(List<int> roleIds, int menuItemId)
        {
            return Collection.Where(p => p.MenuItemId == menuItemId && roleIds.Contains(p.RoleId)).GroupBy(p => p.MenuItemId).ToList().Select(p => new RoleAccess()
            {
                CanView = p.Any(q => q.CanView),
                CanUpdate = p.Any(q => q.CanUpdate),
                CanDelete = p.Any(q => q.CanDelete),
                CanCreate = p.Any(q => q.CanCreate),
                CanAuthorize = p.Any(q => q.CanAuthorize)
            }).FirstOrDefault();
        }
    }
}
