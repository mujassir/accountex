using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class RequisitionItemRepository : GenericRepository<RequisitionItem>
    {
         public RequisitionItemRepository() : base() { }
         public RequisitionItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }



    }
}