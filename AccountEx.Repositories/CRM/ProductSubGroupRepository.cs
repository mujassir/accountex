using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class ProductSubGroupRepository : GenericRepository<ProductSubGroup>
    {
        public ProductSubGroupRepository() : base() { }
        public ProductSubGroupRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public List<IdName> GetIdNameByGroupId(int groupId)
        {
            return Collection.Where(p => p.GroupId == groupId && p.GroupId > 0).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
    }
}