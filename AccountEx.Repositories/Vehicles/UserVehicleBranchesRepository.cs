using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using Entities.CodeFirst;

namespace AccountEx.Repositories
{
    public class UserVehicleBranchesRepository : GenericRepository<UserVehicleBranch>
    {
        public UserVehicleBranchesRepository() : base() { }
        public UserVehicleBranchesRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public List<int> GetBranchIdsByUserIds(int userId)
        {
            return Collection.Where(p => p.UserId == userId).Select(p => p.BranchId).ToList();
        }
        public bool IsUserLinkedWithBranch(int userId)
        {
            return Collection.Any(p => p.UserId == userId);
        }
        public bool IsUserLinkedWithBranch(int userId, int branchId)
        {
            return Collection.Any(p => p.UserId == userId && p.UserId == userId);
        }
        public List<IdName> GetByUserIds(int userId)
        {
            return (from uvb in Collection
                    join vb in AsQueryable<VehicleBranch>() on uvb.BranchId equals vb.Id

                    where uvb.UserId == userId
                    select new IdName()
                    {
                        Id = vb.Id,
                        Name = vb.Name,

                    }).ToList();
        }

        public override void Save(List<UserVehicleBranch> records)
        {
            if (records == null)
                records = new List<UserVehicleBranch>();

            var userId = 0;
            if (records.Any())
                userId = records.FirstOrDefault().UserId;
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = this;
                string query = "Delete from UserVehicleBranches where UserId=" + userId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
                repo.ExecuteQuery(query);
                repo.Add(records);
                repo.SaveChanges();
                scope.Complete();
            }

        }


    }
}
