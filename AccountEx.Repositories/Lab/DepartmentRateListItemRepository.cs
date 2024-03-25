using AccountEx.CodeFirst.Models.Lab;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common.Lab;
using AccountEx.DbMapping.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class DepartmentRateListItemRepository : GenericRepository<DepartmentRateListItem>
    {
        public DepartmentRateListItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
       
    }
}

