using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
namespace AccountEx.Repositories
{
    public class VehicleStatusRepository : GenericRepository<VehicleStatuse>
    {
        public VehicleStatusRepository() : base() { }
        public VehicleStatusRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public int GetFinalStatusId()
        {
            if (Collection.Any(p => p.IsFinal))
                return Collection.FirstOrDefault(p => p.IsFinal).Id;
            else
                return 0;
        }

    }
}
