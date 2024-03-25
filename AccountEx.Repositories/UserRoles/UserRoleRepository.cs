using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using EntityFramework.Extensions;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>
    {
        public UserRoleRepository() : base() { }
        public UserRoleRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public UserRole GetByUserId(int userid)
        {
            return Collection.FirstOrDefault(p => p.UserId == userid);
        }
       
        public List<int> GetRoles(int userId)
        {
            return Collection.Where(p => p.UserId == userId).Select(p => p.RoleId).ToList();
        }
        public bool CheckIfRoleAssigned(int roleid)
        {
            return Collection.Any(p => p.RoleId == roleid);
        }
        public void DeleteByUserId(int userid)
        {
            Db.UserRoles.Delete(p => p.UserId == userid && p.CompanyId == SiteContext.Current.User.CompanyId);

        }


    }
}
