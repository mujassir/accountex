using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class PMCItemRepository : GenericRepository<PMCItem>
    {

        public PMCItemRepository() : base() { }
        public PMCItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public vw_CRMPMCItems GetByIdFromView(int id)
        {
            return AsQueryable<vw_CRMPMCItems>().FirstOrDefault(p => p.Id == id);
        }


    }
}
