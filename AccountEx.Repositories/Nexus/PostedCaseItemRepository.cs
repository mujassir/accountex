using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.Nexus;

namespace AccountEx.Repositories
{
    public class PostedCaseItemRepository : GenericRepository<NexusPostedCasesItems>
    {

        public PostedCaseItemRepository() : base() { }
        public PostedCaseItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        


    }
}
