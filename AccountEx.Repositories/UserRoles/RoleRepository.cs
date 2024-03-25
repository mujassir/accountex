using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository() : base() { }
        public RoleRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public Role GetByName(string name)
        {
            return Collection.FirstOrDefault(p => p.Name == name);
        }
        public int GetDashBoradId(List<int> ids)
        {
            if (Collection.Any(p => ids.Contains(p.Id) && p.DashBoardId != null && p.DashBoardId > 0))
                return Collection.FirstOrDefault(p => ids.Contains(p.Id) && p.DashBoardId != null && p.DashBoardId > 0).DashBoardId.Value;
            else return 0;
        }
        public List<Role> GetByCompanyId(int companyId)
        {
            return Collection.Where(p => p.CompanyId == companyId).ToList();
        }
        public void Save(Role entity, BaseRepository repo)
        {

            if (entity.Id == 0)
            {
                base.Save(entity);
            }
            else
            {
                Update(entity, repo);
            }


        }
        public void Update(Role entity, BaseRepository baseRepo)
        {


            var repo = new RoleRepository(baseRepo);
            var roleAccessRepo = new RoleAccessRepository(baseRepo);
            var roleActionRepo = new RoleActionRepository(baseRepo);
            //var dbRole = GetById(entity.Id, true);

            //add,update & remove role accesses
            //var Ids = entity.RoleAccess.Select(p => p.Id).ToList();
            //var deletedIds = dbRole.RoleAccess.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            //roleAccessRepo.Delete(deletedIds);
            roleAccessRepo.Save(entity.RoleAccess.ToList());

            //add,update & remove role actions
            //Ids = entity.RoleActions.Select(p => p.Id).ToList();
            //deletedIds = dbRole.RoleActions.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            //roleActionRepo.Delete(deletedIds);
            roleActionRepo.Save(entity.RoleActions.ToList());
            base.Update(entity, true, false);



        }

    }
}
