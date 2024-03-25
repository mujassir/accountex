using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class VehicleBranchRepository : GenericRepository<VehicleBranch>
    {
        public VehicleBranchRepository() : base() { }
        public VehicleBranchRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public VehicleBranch GetVehicleBranchById(int? id)
        {
            var vehBr = Collection.FirstOrDefault(p => p.Id == id);
            return vehBr;
        }
        public int GetHeadOfficeId()
        {
            if (Collection.Any(p => p.IsHeadOffice))
                return Collection.FirstOrDefault(p => p.IsHeadOffice).Id;
            else
                return 0;
        }

        public override void Add(VehicleBranch entity)
        {
            if (entity.IsHeadOffice == true)
            {
                if (Collection.AsQueryable().Any(p => p.IsHeadOffice))
                {
                    var vehicleBranch = Collection.AsQueryable().Where(p => p.IsHeadOffice).FirstOrDefault();
                    vehicleBranch.IsHeadOffice = false;
                    Update(vehicleBranch);
                }
            }
            base.Save(entity);
        }
    }
}
