using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class StockRequisitionItemRepository : GenericRepository<StockRequisitionItem>
    {
       public StockRequisitionItemRepository() : base() { }
       public StockRequisitionItemRepository(BaseRepository repo) 
       {
           base.Db = repo.GetContext();
       }
    }
}
