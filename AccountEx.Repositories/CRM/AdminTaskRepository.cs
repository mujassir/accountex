using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common.CRM;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class AdminTaskRepository : GenericRepository<CRMComplaintFile>
    {
        public AdminTaskRepository() : base() { }
        public AdminTaskRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

    }
}