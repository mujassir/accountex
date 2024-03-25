using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class CRMImportRequisitionItemRepository : GenericRepository<CRMImportRequisitionItem>
    {

        public CRMImportRequisitionItemRepository() : base() { }
        public CRMImportRequisitionItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


    }
}
