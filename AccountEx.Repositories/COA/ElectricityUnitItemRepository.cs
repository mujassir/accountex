using AccountEx.CodeFirst.Models.COA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.COA
{
    public class ElectricityUnitItemRepository : GenericRepository<ElectricityUnitItem>
    {
       public ElectricityUnitItemRepository() : base() { }
       public ElectricityUnitItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
  }




