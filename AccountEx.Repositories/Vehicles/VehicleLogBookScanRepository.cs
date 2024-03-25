using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Repositories
{
    public class VehicleLogBookScanRepository : GenericRepository<VehicleLogBookScane>
    {
        public VehicleLogBookScanRepository() : base() { }
        public VehicleLogBookScanRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public List<VehicleLogBookScane> GetByVehicleById(int id)
        {
            return Collection.Where(p => p.VehicleId == id).ToList();
           
        }

        
    }
}
