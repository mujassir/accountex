using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class BLStatusHistoryRepository : GenericRepository<BLStatusHistory>
    {
        public BLStatusHistoryRepository() : base() { }
        public BLStatusHistoryRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public BLStatusHistory GetByBLItemId(int itemId) 
        {
           return Collection.Where(p => p.VehicleId == itemId).FirstOrDefault();
        }

    }
}
