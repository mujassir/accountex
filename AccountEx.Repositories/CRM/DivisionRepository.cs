using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class DivisionRepository : GenericRepository<Division>
    {
        public DivisionRepository() : base() { }
        public DivisionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public List<IdName> GetIdNameByCategoryId(int categoryId)
        {
            return Collection.Where(p => p.CategoryId == categoryId && p.CategoryId > 0).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
    }
}